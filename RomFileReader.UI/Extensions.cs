using MaterialDesignThemes.Wpf;
using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Windows;

namespace RomFileReader.UI
{
    public static class Extensions
    {
        public static IDisposable BindValidationToHelperText<TView, TViewModel, TViewModelProperty>
            (
                this TView @this,
                Expression<Func<TView, TViewModel>> expression,
                TViewModel viewModel,
                Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
                Expression<Func<TView, FrameworkElement>> viewProperty
            )
            where TViewModel : IReactiveObject, IValidatableViewModel
        {
            var frameworkElement = viewProperty.Compile().Invoke(@this);

            return @this.WhenAnyValue(expression)
                .Select(vm => vm!.ValidationContext.Validations
                    // Observe to add ValidationRule
                    .ToObservable()
                    .Select(_ => viewModel // Get the corresponding Validation Component
                        .ValidationContext.Validations
                        .OfType<IPropertyValidationComponent>()
                        .Where(x => x.ContainsProperty(viewModelProperty))
                        // Combining ValidationStatusChange
                        .Select(x => x.ValidationStatusChange)
                        .CombineLatest())
                    .Switch())
                .Switch()
                .Select(validations =>
                    string.Join(
                        Environment.NewLine,
                        validations
                            .Where(x => !x.IsValid)
                            .SelectMany(x => x.Text)))
                .DistinctUntilChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(validationText =>
                {
                    HintAssist.SetHelperText(frameworkElement, validationText);
                });
        }
    }
}
