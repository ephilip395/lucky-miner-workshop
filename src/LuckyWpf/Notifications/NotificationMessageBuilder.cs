using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Lucky.Notifications
{
    /// <summary>
    /// The notification message builder.
    /// </summary>
    public class NotificationMessageBuilder
    {
        public static NotificationMessageBuilder CreateMessage(INotificationMessageManager manager)
        {
            return new NotificationMessageBuilder(manager);
        }

        public INotificationMessageManager Manager { get; private set; }

        public INotificationMessage Message { get; private set; }

        private NotificationMessageBuilder(INotificationMessageManager manager)
        {
            Manager = manager;
            Message = manager.Factory.GetMessage();
        }


        public void SetHeader(string header)
        {
            Message.Header = header;
        }

        public void SetMessage(string message)
        {
            Message.Message = message;
        }

        public void AddButton(INotificationMessageButton button)
        {
            if (button == null)
            {
                throw new ArgumentNullException(nameof(button));
            }

            Message.Buttons.Add(button);
        }

        public void SetBadge(string badgeText)
        {
            Message.BadgeText = badgeText;
        }

        public void SetAccent(Brush accentBrush)
        {
            Message.AccentBrush = accentBrush;
        }

        public void SetBackground(Brush backgroundBrush)
        {
            Message.Background = backgroundBrush;
        }

        public void SetOverlay(object overlay)
        {
            Message.OverlayContent = overlay;
        }

        public void SetAdditionalContentTop(object additionalContentTop)
        {
            Message.AdditionalContentTop = additionalContentTop;
        }

        public void SetAdditionalContentBottom(object additionalContentBottom)
        {
            Message.AdditionalContentBottom = additionalContentBottom;
        }

        public void SetAdditionalContentLeft(object additionalContentLeft)
        {
            Message.AdditionalContentLeft = additionalContentLeft;
        }

        public void SetAdditionalContentRight(object additionalContentRight)
        {
            Message.AdditionalContentRight = additionalContentRight;
        }

        public void SetAdditionalContentMain(object additionalContentMain)
        {
            Message.AdditionalContentMain = additionalContentMain;
        }

        public void SetAdditionalContentOverBadge(object additionalContentOverBadge)
        {
            Message.AdditionalContentOverBadge = additionalContentOverBadge;
        }

        public void SetForeground(Brush brush)
        {
            Message.Foreground = brush;
        }

        public void SetAnimates(bool animates)
        {
            if (Message is INotificationAnimation animation)
            {
                animation.Animates = animates;
            }
        }

        public void SetAnimationInDuration(double duration)
        {
            if (Message is INotificationAnimation animation)
            {
                animation.AnimationInDuration = duration;
            }
        }

        public void SetAnimationOutDuration(double duration)
        {
            if (Message is INotificationAnimation animation)
            {
                animation.AnimationOutDuration = duration;
            }
        }

        public void SetAnimationIn(AnimationTimeline animation)
        {
            if (Message is INotificationAnimation notificationAnimation)
            {
                notificationAnimation.AnimationIn = animation;
            }
        }

        public void SetAnimationOut(AnimationTimeline animation)
        {
            if (Message is INotificationAnimation notificationAnimation)
            {
                notificationAnimation.AnimationOut = animation;
            }
        }

        public void SetAnimationInDependencyProperty(DependencyProperty property)
        {
            if (Message is INotificationAnimation animation)
            {
                animation.AnimationInDependencyProperty = property;
            }
        }

        public void SetAnimationOutDependencyProperty(DependencyProperty property)
        {
            if (Message is INotificationAnimation animation)
            {
                animation.AnimationOutDependencyProperty = property;
            }
        }

        public INotificationMessage Queue()
        {
            Manager.Queue(Message);

            return Message;
        }

        public void Delay(int seconds, Action<INotificationMessage> action)
        {
            _ = seconds.SecondsDelay().ContinueWith(
                context => action(Message),
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Executes the action after specified delay time.
        /// </summary>
        /// <param name="delay">The delay.</param>
        /// <param name="action">The action.</param>
        public void Delay(TimeSpan delay, Action<INotificationMessage> action)
        {
            _ = delay.Delay().ContinueWith(
                context => action(Message),
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// The notification message button that is required to dismiss the notification.
        /// </summary>
        public class DismissNotificationMessage
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DismissNotificationMessage"/> class.
            /// </summary>
            /// <param name="builder">The builder.</param>
            /// <exception cref="ArgumentNullException">builder</exception>
            public DismissNotificationMessage(NotificationMessageBuilder builder)
            {
                Builder = builder ?? throw new ArgumentNullException(nameof(builder));
            }

            /// <summary>
            /// Gets or sets the builder.
            /// </summary>
            /// <value>
            /// The builder.
            /// </value>
            public NotificationMessageBuilder Builder { get; private set; }
        }
    }
}