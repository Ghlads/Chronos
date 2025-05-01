using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Framework.Core.Editor
{
    [CustomPropertyDrawer( typeof( ReadonlyAttribute ) )]
    public class ReadonlyAttributeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI( SerializedProperty property )
        {
            VisualElement element = new PropertyField( property );
            element.SetEnabled( false );
            element.AddToClassList( "readonly" );
            return element;
        }
    }
}
