using MotoRapido.Models;
using Xamarin.Forms;

namespace MotoRapido.Customs.Cells
{
    public class SelectorDataTemplate : DataTemplateSelector
    {

        private readonly DataTemplate textInDataTemplate;

        private readonly DataTemplate textOutDataTemplate;

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var messageVm = item as Message;
            if (messageVm == null)
                return null;
            return messageVm.IsTextIn ? this.textInDataTemplate : this.textOutDataTemplate;
        }


        public SelectorDataTemplate()
        {
            this.textInDataTemplate = new DataTemplate(typeof(TextInViewCell));
            this.textOutDataTemplate = new DataTemplate(typeof(TextOutViewCell));
        }

    }
}
