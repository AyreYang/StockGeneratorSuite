
namespace SGNativeEntities.Enums
{
    public enum MARKET
    {
        none,
        sh,
        sz
    }
    public enum CODETYPE
    {
        index, stock
    }
    public enum TRADE
    {
        none = 99,
        sale = -1,
        middle = 0,
        buy = 1,
    }
    public enum TABLES
    {
        NONE,
        IDX_DAILY_TD,
        IDX_DAILY_TD_SEQ,
        IDX_REALTIME_TD,
        //IDX_REALTIME_TD_SEQ,
        IDX_GENERAL_M,

        STK_DAILY_TD,
        STK_MINUTE_TD,
        //STK_DAILY_TD_SEQ,
        STK_REALTIME_TD,
        //STK_REALTIME_TD_SEQ,
        STK_GENERAL_M,
        STK_MACD_TD,
        STK_RSI_TD,

        STK_SUM_RESULT_TD,

        STK_FAVORITE_M
    }
}
