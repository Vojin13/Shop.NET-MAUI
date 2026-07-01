namespace Android_Ispit.Behaviors
{
    // Entry/Editor/Picker have no BorderColor of their own, so form fields wrap them in a
    // Border and this behavior recolors that Border's Stroke on focus/unfocus instead.
    public class FocusBorderBehavior : Behavior<Border>
    {
        private EventHandler<FocusEventArgs>? _focusedHandler;
        private EventHandler<FocusEventArgs>? _unfocusedHandler;

        protected override void OnAttachedTo(Border bindable)
        {
            base.OnAttachedTo(bindable);
            if (bindable.Content is VisualElement content)
            {
                _focusedHandler = (s, e) => bindable.Stroke = GetColor("Primary");
                _unfocusedHandler = (s, e) => bindable.Stroke = GetColor("AccentSoft");
                content.Focused += _focusedHandler;
                content.Unfocused += _unfocusedHandler;
            }
        }

        protected override void OnDetachingFrom(Border bindable)
        {
            base.OnDetachingFrom(bindable);
            if (bindable.Content is VisualElement content)
            {
                if (_focusedHandler != null) content.Focused -= _focusedHandler;
                if (_unfocusedHandler != null) content.Unfocused -= _unfocusedHandler;
            }
        }

        private static Color GetColor(string key)
        {
            return Application.Current?.Resources.TryGetValue(key, out var value) == true && value is Color color
                ? color
                : Colors.Gray;
        }
    }
}
