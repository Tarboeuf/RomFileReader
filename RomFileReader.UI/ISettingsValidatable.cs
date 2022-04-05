using ReactiveUI;
using ReactiveUI.Validation.Abstractions;

namespace RomFileReader.UI
{
    public interface ISettingsValidatable : ISettings, IValidatableViewModel, IReactiveObject
    {

    }
}
