using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SharpCaster.Simple.Controllers
{
    public sealed class CastPlayer : Control
    {
        public static readonly DependencyProperty PosterImageSourceProperty = DependencyProperty.Register(
            "PosterImageSource", typeof (ImageSource), typeof (CastPlayer), new PropertyMetadata(default(ImageSource)));

        public ImageSource PosterImageSource
        {
            get { return (ImageSource) GetValue(PosterImageSourceProperty); }
            set { SetValue(PosterImageSourceProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof (string), typeof (CastPlayer), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            "Description", typeof (string), typeof (CastPlayer), new PropertyMetadata(default(string)));

        public string Description
        {
            get { return (string) GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }
        public CastPlayer()
        {
            this.DefaultStyleKey = typeof(CastPlayer);
        }
    }
}
