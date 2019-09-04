#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>


@interface IAPManager : NSObject<SKProductsRequestDelegate, SKPaymentTransactionObserver>

@end