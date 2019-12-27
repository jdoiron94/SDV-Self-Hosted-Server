namespace SelfHostedServer
{
    class ModConfig
    {

        public bool Debug { get; set; } = false;
        public Save SaveData { get; set; } = null;
    }

    class Save
    {
        public string FileName { get; set; } = null;
        public bool Multiplayer { get; set; } = true;
    }
}