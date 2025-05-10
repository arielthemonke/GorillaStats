namespace GorillaStats
{
    public abstract class GorillaStatsPlugin
    {
        public bool isEnabled { get; private set; }
        public virtual void OnEnable()
        {
            isEnabled = true;
        }

        public virtual void OnDisable()
        {
            isEnabled = false;
        }

        public virtual void Forever()
        {
            
        }
        
        public abstract string TextToDisplay { get; }
    }
}