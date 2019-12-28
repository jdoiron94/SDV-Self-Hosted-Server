namespace SelfHostedServer
{
    class ModConfig
    {
        public int TimeToSleep { get; set; } = 630;
        public Save SaveData { get; set; } = null;
    }

    class Save
    {
        public string FileName { get; set; } = null;
        public bool Multiplayer { get; set; } = true;
    }
}