local BaseCtrl = class("BaseCtrl")

function BaseCtrl:ctor( name )
	self.name = name
	tools.PrintDebug("-----------初始化BaseCtrl", self.name, self)
end

function BaseCtrl:Awake()
	if self.creating then
		return
	end
	self.active = true;
	tools.PrintDebug("----------显示ＵＩ,预设名字：", self.name, self.inited, self.view)
	if not self.inited then
		self.creating = true
		panelMgr:CreatePanel("Prefabs/" .. self.name, self, self.CreateView)
	else
		self.view.gameObject:SetActive(true)
		self.view.transform.localPosition = Vector3.zero
		self.view:UpdateView()
	end
	
end

function BaseCtrl:CreateView(obj)
	self:OnCreate(obj)
	self.creating = false
    self.isCreate = true
end

function BaseCtrl:OnCreate()
	-- body
end

function BaseCtrl:UpdateView()
	self.view.transform.localPosition = Vector3.zero
	self.view.gameObject:SetActive(true)
	self.view:UpdateView()
end

function BaseCtrl:SetData(data)
end

function BaseCtrl:InitPanel( ... )
	-- body
end

function BaseCtrl:OnDestroy()
	self:Close()
	self.inited = false
    self.isCreate = false
	self.view:OnDestroy()
	self = nil
end

function BaseCtrl:Close()
	self.active = false
	self.view:Close()
end

return BaseCtrl