using System.Windows;

namespace Appbyfex.Validators
{
    /// <summary>
    /// Extensions for UIElements
    /// </summary>
    public static class UIElementExtensions
    {
        /// <summary>
        /// Sets a custom validation delegate on a given UIElement.
        /// </summary>
        /// <param name="uiElement"></param>
        /// <param name="validationDelegate"></param>
        public static void SetCustomValidation(this UIElement uiElement, ValidationDelegate validationDelegate)
        {
            // Set the validation delegate on the uiElement
            uiElement.SetValue(ValidatorService.ValidateProperty, validationDelegate);
        }
    }
}
