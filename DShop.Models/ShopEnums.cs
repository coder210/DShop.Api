namespace DShop.Models
{
    /// <summary>
    /// 商品SPU状态
    /// </summary>
    public enum SpuStatus
    {
        /// <summary>上架</summary>
        PutOnShelves,
        /// <summary>下架</summary>
        PutOffShelves
    }

    /// <summary>
    /// 分类状态
    /// </summary>
    public enum CategoryStatus
    {
        /// <summary>正常</summary>
        Normal,
        /// <summary>隐藏</summary>
        Hide
    }

    /// <summary>
    /// 品牌状态
    /// </summary>
    public enum BrandStatus
    {
        /// <summary>正常</summary>
        Normal,
        /// <summary>隐藏</summary>
        Hide
    }

    /// <summary>
    /// 属性是否需要检索
    /// </summary>
    public enum AttrSearchType
    {
        /// <summary>不需要</summary>
        Unwanted,
        /// <summary>需要</summary>
        Need
    }

    /// <summary>
    /// 属性值类型
    /// </summary>
    public enum AttrValueType
    {
        /// <summary>单个值</summary>
        Single,
        /// <summary>多个值</summary>
        Multiple
    }

    /// <summary>
    /// 属性类型
    /// </summary>
    public enum AttrType
    {
        /// <summary>销售属性</summary>
        Sale,
        /// <summary>基本属性</summary>
        Base,
        /// <summary>两者都是</summary>
        Both
    }

    /// <summary>
    /// 属性状态
    /// </summary>
    public enum AttrStatus
    {
        /// <summary>启用</summary>
        Enable,
        /// <summary>禁用</summary>
        Disable
    }

    /// <summary>
    /// 订单状态
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>待付款</summary>
        PendingPayment,
        /// <summary>待发货</summary>
        PendingShipment,
        /// <summary>已发货</summary>
        Shipped,
        /// <summary>待评价</summary>
        PendingEvaluation,
        /// <summary>已完成</summary>
        Finished,
        /// <summary>已关闭</summary>
        Closed,
        /// <summary>无效订单</summary>
        InvalidOrder
    }

    /// <summary>
    /// 支付方式【1->支付宝；2->微信；3->银联；4->货到付款】
    /// </summary>
    public enum OrderPayType
    {
        /// <summary>未知</summary>
        Unknown,
        /// <summary>支付宝</summary>
        Alipay,
        /// <summary>微信</summary>
        Wechat,
        /// <summary>银联</summary>
        UnionPay,
        /// <summary>货到付款</summary>
        CashOnDelivery
    }

    /// <summary>
    /// 订单来源
    /// </summary>
    public enum OrderSourceType
    {
        /// <summary>H5</summary>
        H5,
        /// <summary>App</summary>
        App
    }

    /// <summary>
    /// 订单事件类型
    /// </summary>
    public enum OrderEventType
    {
        /// <summary>下单失败</summary>
        OrderFailed,
        /// <summary>支付订单</summary>
        PayOrder
    }

    /// <summary>
    /// 订单事件状态
    /// </summary>
    public enum OrderEventStatus
    {
        /// <summary>未处理</summary>
        Unprocessed,
        /// <summary>已处理</summary>
        Processed
    }

    /// <summary>
    /// 发票类型
    /// </summary>
    public enum BillType
    {
        /// <summary>不开发票</summary>
        NoReceipt,
        /// <summary>电子发票</summary>
        ElectronicInvoice,
        /// <summary>纸质发票</summary>
        PaperInvoice
    }

    /// <summary>
    /// 客户性别
    /// </summary>
    public enum CustomerGender
    {
        /// <summary>未知</summary>
        Unknown,
        /// <summary>男</summary>
        Male,
        /// <summary>女</summary>
        Female
    }

    /// <summary>
    /// 客户状态
    /// </summary>
    public enum CustomerStatus
    {
        /// <summary>启用</summary>
        Enable,
        /// <summary>禁用</summary>
        Disable
    }

    /// <summary>
    /// 积分流水类型
    /// </summary>
    public enum CoinRecordType
    {
        /// <summary>增加</summary>
        Increase,
        /// <summary>减少</summary>
        Subtract
    }

    /// <summary>
    /// 验证码类型
    /// </summary>
    public enum IdentifyingCodeType
    {
        /// <summary>登录</summary>
        Login,
        /// <summary>注册</summary>
        Register,
        /// <summary>忘记密码</summary>
        ForgetPassword
    }

    /// <summary>
    /// 验证码状态
    /// </summary>
    public enum IdentifyingCodeStatus
    {
        /// <summary>未使用</summary>
        Unused,
        /// <summary>已使用</summary>
        Used
    }
}
