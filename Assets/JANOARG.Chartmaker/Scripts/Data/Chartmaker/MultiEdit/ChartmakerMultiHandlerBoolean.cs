namespace JANOARG.Chartmaker.Data.Chartmaker.MultiEdit
{
    public class ChartmakerMultiHandlerBoolean: ChartmakerMultiHandler<bool>
    {
        public new bool? To;
    
        public override bool Get(bool from, object source, int index = 0) 
            => To ?? !from;
    }
}