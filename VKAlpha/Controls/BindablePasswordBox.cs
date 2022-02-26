using System.Windows;
using System.Windows.Controls;

namespace VKAlpha.Controls
{
    public sealed class BindablePasswordBox : Decorator
    {
        /// <summary>
        /// The password dependency property.
        /// </summary>
        public static readonly DependencyProperty PasswordProperty;
        public static readonly DependencyProperty ForegroundProperty;

        private bool _isPreventCallback;
        private readonly RoutedEventHandler _savedCallback;

        /// <summary>
        /// Static constructor to initialize the dependency properties.
        /// </summary>
        static BindablePasswordBox()
        {
            PasswordProperty = DependencyProperty.Register(
                "Password",
                typeof(string),
                typeof(BindablePasswordBox),
                new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnPasswordPropertyChanged))
            );
            ForegroundProperty = DependencyProperty.Register(
                "Foreground",
                typeof(System.Windows.Media.Brush),
                typeof(BindablePasswordBox),
                new PropertyMetadata(default(System.Windows.Media.Brush), new PropertyChangedCallback(OnForegroundPropertyChanged))
            );
        }

        /// <summary>
        /// Saves the password changed callback and sets the child element to the password box.
        /// </summary>
        public BindablePasswordBox()
        {
            _savedCallback = HandlePasswordChanged;

            var passwordBox = new PasswordBox { Style = Application.Current.Resources["MaterialDesignPasswordBox"] as Style };
            passwordBox.PasswordChanged += _savedCallback;
            Child = passwordBox;
        }

        public System.Windows.Media.Brush Foreground
        {
            get => GetValue(ForegroundProperty) as System.Windows.Media.Brush;
            set => SetValue(ForegroundProperty, value);
        }

        /// <summary>
        /// The password dependency property.
        /// </summary>
        public string Password
        {
            get { return GetValue(PasswordProperty) as string; }
            set { SetValue(PasswordProperty, value); }
        }

        private static void OnForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var bindablePasswordBox = (BindablePasswordBox)d;
            var passwordBox = (PasswordBox)bindablePasswordBox.Child;

            if (bindablePasswordBox._isPreventCallback)
            {
                return;
            }

            passwordBox.Foreground = (System.Windows.Media.Brush)((args.NewValue != null) ? args.NewValue : default);
        }

        /// <summary>
        /// Handles changes to the password dependency property.
        /// </summary>
        /// <param name="d">the dependency object</param>
        /// <param name="eventArgs">the event args</param>
        private static void OnPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs eventArgs)
        {
            var bindablePasswordBox = (BindablePasswordBox)d;
            var passwordBox = (PasswordBox)bindablePasswordBox.Child;

            if (bindablePasswordBox._isPreventCallback)
            {
                return;
            }

            passwordBox.PasswordChanged -= bindablePasswordBox._savedCallback;
            passwordBox.Password = (eventArgs.NewValue != null) ? eventArgs.NewValue.ToString() : "";
            passwordBox.PasswordChanged += bindablePasswordBox._savedCallback;
        }

        /// <summary>
        /// Handles the password changed event.
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="eventArgs">the event args</param>
        private void HandlePasswordChanged(object sender, RoutedEventArgs eventArgs)
        {
            var passwordBox = (PasswordBox)sender;

            _isPreventCallback = true;
            Password = passwordBox.Password;
            _isPreventCallback = false;
        }
    }
}
