namespace FakeLivingComments.Config
{
    /// <summary>
    /// 配置结构体
    /// </summary>
    public struct ConfigStruct
    {
        /// <summary>
        /// 每条弹幕停留在屏幕上的时间，单位为秒
        /// </summary>
        public float CommentStaySeconds;
        /// <summary>
        /// 弹幕的不透明度，0-1
        /// </summary>
        public float CommentAlpha;
        /// <summary>
        /// 弹幕字体大小乘数，基于窗口尺寸的高度。值越大，弹幕字体越大
        /// </summary>
        public float CommentFontSizeMulti;
        /// <summary>
        /// 允许同时存在的最大弹幕条数
        /// </summary>
        public int CommentMaxCount;
        /// <summary>
        /// 预备弹幕的最大条数
        /// </summary>
        public int ReserveMaxCount;
        /// <summary>
        /// 弹幕出现在屏幕上的最低位置乘数，基于窗口尺寸的高度，填写0-1。此值越大，弹幕最低位置越接近屏幕顶部，反之若为0，则允许弹幕出现在画面的任意高度
        /// </summary>
        public float CommentLowestHeight;
        /// <summary>
        /// 弹幕字体边框宽度乘数，用于TMP_Text.outlineWidth，是字体大小的乘数
        /// </summary>
        public float CommentOutlineWidth;
        /// <summary>
        /// 过滤器命令执行的最大允许命令数
        /// </summary>
        public int FilterExecutionTTL;
    }
}