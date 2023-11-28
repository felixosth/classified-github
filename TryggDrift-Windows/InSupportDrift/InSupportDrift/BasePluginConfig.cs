using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace InSupport.Drift.Plugins
{
    [TypeDescriptionProvider(typeof(AbstractControlDescriptionProvider<BasePluginConfig, UserControl>))]
    public abstract partial class BasePluginConfig : UserControl
    {
        public BasePluginConfig()
        {
            InitializeComponent();
        }

        public abstract bool ValidateForm();

        public abstract string[] GetSettings();
    }

    public class AbstractControlDescriptionProvider<TAbstract, TBase> : TypeDescriptionProvider
    {
        public AbstractControlDescriptionProvider()
            : base(TypeDescriptor.GetProvider(typeof(TAbstract)))
        {
        }

        public override Type GetReflectionType(Type objectType, object instance)
        {
            if (objectType == typeof(TAbstract))
                return typeof(TBase);

            return base.GetReflectionType(objectType, instance);
        }

        public override object CreateInstance(IServiceProvider provider, Type objectType, Type[] argTypes, object[] args)
        {
            if (objectType == typeof(TAbstract))
                objectType = typeof(TBase);

            return base.CreateInstance(provider, objectType, argTypes, args);
        }
    }
}
