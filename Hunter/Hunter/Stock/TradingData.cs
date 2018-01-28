using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunter
{
    public class TradingData
    {
        public Stock stock;
        public TradingRunData runData;
        public Deal deal;
        public StrategyData dmData;
        public StrategyData buy_dmData;

        public TradingData()
        {
            runData = new TradingRunData();
            //deal = new Position();
            dmData = new StrategyData();
            buy_dmData = new StrategyData();
            //sell_dmData = new DmData();
        }

        public void SetData(Stock _stock)
        {
            stock = _stock;
            runData = new TradingRunData(stock.runData);
            //deal = new Position(stock.position);
            dmData = new StrategyData(stock.dmData);
            buy_dmData = new StrategyData(stock.buy_dmData);

            //stock.position = new Position(stock);
        }
    }


}
