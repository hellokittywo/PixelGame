#import "IAPManager.h"
#import "SSKeychain.h"

@implementation IAPManager
#define kService [NSBundle mainBundle].bundleIdentifier
#define kAccount @"1406648225.com.TianHeng.NewGiftsFree"
-(void)addObserver{
    [[SKPaymentQueue defaultQueue] addTransactionObserver:self];
}
-(void)getProductInfo{
    NSSet * set = [NSSet setWithArray:@[productID]];
    SKProductsRequest * request = [[SKProductsRequest alloc] initWithProductIdentifiers:set];
    request.delegate = self;
    [request start];
}
- (void)checkPay:(NSString *)payload
{
    NSData *payloadData =[payload dataUsingEncoding:NSUTF8StringEncoding];
    // 发送网络POST请求，对购买凭据进行验证
    NSString *verifyUrlString;
    //verifyUrlString = @"https://sandbox.itunes.apple.com/verifyReceipt";
    verifyUrlString = @"https://buy.itunes.apple.com/verifyReceipt";
    // 国内访问苹果服务器比较慢，timeoutInterval 需要长一点
    
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:[[NSURL alloc] initWithString:verifyUrlString] cachePolicy:NSURLRequestUseProtocolCachePolicy timeoutInterval:10.0f];
    [request setHTTPMethod:@"POST"];
    [request setHTTPBody:payloadData];
    NSData*result =[NSURLConnection sendSynchronousRequest:request returningResponse:nil error:nil];
    if(result ==nil) {
    }
    NSDictionary*dict =[NSJSONSerialization JSONObjectWithData:result options:NSJSONReadingAllowFragments error:nil];
    if(dict !=nil) {
        NSString *str = [dict objectForKey:@"status"];
        int state = [str intValue];
        NSDictionary *data = [dict objectForKey:@"receipt"];
        NSString *bundle = [data objectForKey:@"bundle_id"];
        NSArray *array = [data objectForKey:@"in_app"];
        NSDictionary *appdata = array[0];
        NSString *pro = [appdata objectForKey:@"product_id"];
        NSString *string2 = [pro substringToIndex:2];
        if(state == 0 && [bundle isEqualToString: @"com.TianHeng.NewGiftsFree"] && [string2 isEqualToString: @"gt"])
        {
            [SSKeychain deletePasswordForService:kService account:kAccount];
            UnitySendMessage("PaymentManager", "PayMoneySuccess", [pro UTF8String]);
            //NSLog(@"验证成功", pro);
        }
    }
}

static NSString * productID;
static IAPManager* iapManager;
static NSString * callbackGameObject;
static NSString * callbackFunc;
static int iapID;
extern "C"{
    void _iosPay(const char* gameObjectName,
        const char* funcName, const char* productId,
        int payId, const char* extendJsonData)
    {
        callbackGameObject = [NSString stringWithUTF8String: gameObjectName];
        callbackFunc = [NSString stringWithUTF8String: funcName];
        productID = [NSString stringWithUTF8String: productId];
        iapID = payId;
        if(iapManager == NULL)
        {
            iapManager = [[IAPManager alloc] init];
            [iapManager addObserver];
        }
        else
        {
            [iapManager addObserver];
        }
        if ([SKPaymentQueue canMakePayments])
        {
            [iapManager getProductInfo];
        }
        else
        {
            NSLog(@"用户禁止应用内付费购买");
        }
    }
    
    void _getIosToken()
    {
        if([SSKeychain passwordForService:kService account:kAccount])
        {
            NSString *payload = [SSKeychain passwordForService:kService account:kAccount];
            if(iapManager == NULL)
            {
                iapManager = [[IAPManager alloc] init];
            }
            [iapManager checkPay:payload];
        }
    }
    void _toAppStore()
    {
    }
    void _saveAccount(const char* name)
    {
    }
}


// 以上查询的回调函数
- (void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response {
    NSArray *myProduct = response.products;
    if (myProduct.count == 0) {
        NSLog(@"无法获取产品信息，购买失败。");
        return;
    }
    NSArray* transactions = [SKPaymentQueue defaultQueue].transactions;
    if (transactions.count > 0) {
        //检测是否有未完成的交易
        SKPaymentTransaction* transaction = [transactions firstObject];
        if (transaction.transactionState == SKPaymentTransactionStatePurchased) {
            [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
            return;
        }
    }
    SKMutablePayment * payment = nil;
    payment = [SKMutablePayment paymentWithProduct:myProduct[0]];
    payment.applicationUsername = [NSString stringWithFormat:@"%d",iapID];
    
    [[SKPaymentQueue defaultQueue] addPayment:payment];
    
}

- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions {
    for (SKPaymentTransaction *transaction in transactions)
    {
        switch (transaction.transactionState)
        {
            case SKPaymentTransactionStatePurchased://交易完成
                //                NSLog(@"transactionIdentifier = %@", transaction.transactionIdentifier);
                [self completeTransaction:transaction];
                break;
            case SKPaymentTransactionStateFailed://交易失败
                [self failedTransaction:transaction];
                break;
            case SKPaymentTransactionStateRestored://已经购买过该商品
                [self restoreTransaction:transaction];
                break;
            case SKPaymentTransactionStatePurchasing:      //商品添加进列表
                //                NSLog(@"商品添加进列表");
                break;
            default:
                break;
        }
    }
}
- (void)completeTransaction:(SKPaymentTransaction *)transaction {
    // Your application should implement these two methods.
    NSString * productIdentifier = transaction.payment.productIdentifier;
    [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
    if ([productIdentifier length] > 0) {
        [self verifyTransactionResult];
        // // 向自己的服务器验证购买凭证
        // NSString *temptransactionReceipt  = [[NSString alloc] initWithData:transaction.transactionReceipt encoding:NSUTF8StringEncoding];
        // //item["receipt_data"] = CCLuaValue::stringValue([temptransactionReceipt UTF8String]);
        // //item["productIdentifier"] = CCLuaValue::stringValue([transaction.transactionIdentifier UTF8String]);
        // NSMutableDictionary *dictRoot = [NSMutableDictionary dictionaryWithCapacity:2];
        // NSString * iapid = transaction.payment.applicationUsername;
        // if (iapid == nil)
        // {
        //     iapid = [NSString stringWithFormat:@"%d",iapID];
        // }
        // [dictRoot setObject:iapid forKey:@"iapId"];
        // [dictRoot setObject:temptransactionReceipt forKey:@"receipt_data"];
        
        // NSError *error;
        // NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dictRoot options:kNilOptions error:&error];
        // NSString *jsonStr =[[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        // UnitySendMessage("PaymentManager",
        //     "PayMoneySuccess",
        //     [jsonStr UTF8String]);
    }
    // Remove the transaction from the payment queue.
}

- (void)verifyTransactionResult
{
    /**验证凭据，获取到苹果返回的交易凭据
     appStoreReceiptURL iOS7.0增加的，购买交易完成后，会将凭据存放在该地址
     BASE64 常用的编码方案，通常用于数据传输，以及加密算法的基础算法，传输过程中能够保证数据传输的稳定性
    */
    NSURL *url = [[NSBundle mainBundle] appStoreReceiptURL];
    // 从沙盒中获取到购买凭据
    NSData *receiptData = [NSData dataWithContentsOfURL:url];
    NSDictionary * contents = @{
                                @"receipt-data":[receiptData base64EncodedDataWithOptions:0]
                                };
    NSError *error;
    NSString *encodeStr =[receiptData base64EncodedStringWithOptions:NSDataBase64EncodingEndLineWithLineFeed];
    NSString *payload = [NSString stringWithFormat:@"{\"receipt-data\" : \"%@\"}", encodeStr];
    
    [SSKeychain setPassword:payload forService:kService account:kAccount];
    [self checkPay:payload];
}

- (void)failedTransaction:(SKPaymentTransaction *)transaction {
    if(transaction.error.code != SKErrorPaymentCancelled) {
        //NSLog(@"购买失败");
        UnitySendMessage("PaymentManager",
                         "PayMoneyFailed",
                         [@"-1" UTF8String]);
    } else {
        //NSLog(@"用户取消交易");
        UnitySendMessage("PaymentManager",
                         "PayMoneyFailed",
                         [@"-2" UTF8String]);
    }
    [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
}
- (void)restoreTransaction:(SKPaymentTransaction *)transaction {
    // 对于已购商品，处理恢复购买的逻辑
    [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
}


@end
