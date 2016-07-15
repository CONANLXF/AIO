using VayneHunter_Reborn.Modules.ModuleHelpers;

using TargetSelector = PortAIO.TSManager; namespace VayneHunter_Reborn.Modules
{
    interface IModule
    {
        void OnLoad();

        bool ShouldGetExecuted();

        ModuleType GetModuleType();

        void OnExecute();
    }
}
