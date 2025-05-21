using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using static Framework.Core.StateMachine<Framework.Core.AnyValue.ValueType>;

namespace Framework.Core.Editor
{

    [CustomPropertyDrawer( typeof( AnyValue ) )]
    public class AnyValueDrawe : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI( SerializedProperty property )
        {
            VisualElement root = new VisualElement();
            root.Add( new Label( property.displayName ) );
            root.Add( AnyValueField.FullControlField( property ) );
            root.style.flexDirection = FlexDirection.Row;
            root.style.justifyContent = Justify.SpaceBetween;
            root.style.alignItems = Align.Center;
            return root;
        }
    }


    public class AnyValueField : VisualElement
    {
        public const string ELEMENT_CLASS = "any-value-field";
        public const string TYPE_FIELD_CLASS = ELEMENT_CLASS + "__value-type-field";
        public const string VALUE_FIELD_CLASS = ELEMENT_CLASS + "__value-field";
        public const string BOOL_CLASS = VALUE_FIELD_CLASS + "__bool";
        public const string INT_CLASS = VALUE_FIELD_CLASS + "__int";
        public const string FLOAT_CLASS = VALUE_FIELD_CLASS + "__float";
        public const string VECTOR2_CLASS = VALUE_FIELD_CLASS + "__vector2";
        public const string VECTOR3_CLASS = VALUE_FIELD_CLASS + "__vector3";
        public const string VECTOR4_CLASS = VALUE_FIELD_CLASS + "__vector4";
        public const string RECT_CLASS = VALUE_FIELD_CLASS + "__rect";
        public const string COLOR_CLASS = VALUE_FIELD_CLASS + "__color";
        public const string STRING_CLASS = VALUE_FIELD_CLASS + "__string";
        public const string OBJECT_CLASS = VALUE_FIELD_CLASS + "__object";

        private enum Mode
        {
            FullControl = 0,
            Restricted = 1,
        }


        private class TypeState : State
        {
            public TypeState( AnyValue.ValueType id, VisualElement[] elements ) :
            base(
                id,
                _ => elements.Foreach( element => element.style.display = DisplayStyle.Flex ),
                _ => elements.Foreach( element => element.style.display = DisplayStyle.None ),
                null
                ) {}
        }


        // Type
        private readonly EnumField m_valueTypeField;
        private readonly SerializedProperty m_valueTypeProperty;
        // Type

        // Value
        private readonly SerializedProperty m_stringProperty;
        private readonly SerializedProperty m_xProperty, m_yProperty, m_zProperty, m_wProperty;
        // Field
        private readonly TextField m_stringField;
        private readonly Toggle m_boolField;
        private readonly IntegerField m_intField;
        private readonly FloatField m_floatField;
        private readonly Vector2Field m_vector2Field;
        private readonly Vector3Field m_vector3Field;
        private readonly Vector4Field m_vector4Field;
        private readonly RectField m_rectField;
        private readonly ColorField m_colorField;
        // Field
        // Value

        // Unity Object
        private readonly SerializedProperty m_objectProperty;
        // Field
        private readonly ObjectField m_objectField;
        // Field
        // Unity Object

        private readonly Mode m_mode;

        private readonly StateMachine<AnyValue.ValueType> m_stateMachine;

        public static AnyValueField FullControlField( SerializedProperty property )
        {
            return new( property, Mode.FullControl );
        }


        public static AnyValueField RestrictedField( SerializedProperty property, Type restrictedType )
        {
            property.FindPropertyRelative( "m_type" ).enumValueIndex = ( int )AnyValueUtils.TypeToEnum( restrictedType );
            property.ApplyModificationAndUpdate();
            return new( property, Mode.Restricted, restrictedType );
        }


        private AnyValueField( SerializedProperty property, Mode mode, Type restrictedType = null )
        {
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>( "Assets/Framework/Script/Core/Editor/ScriptingAction/USS/ScriptingActionSheet.uss" );
            styleSheets.Add( styleSheet );

            if ( restrictedType == null || !restrictedType.InheritsFrom<UnityEngine.Object>() )
            {
                restrictedType = typeof( UnityEngine.Object );
            }

            m_mode = mode;

            // Property
            m_valueTypeProperty = property.FindPropertyRelative( "m_type" );
            m_stringProperty = property.FindPropertyRelative( "m_stringValue" );
            m_xProperty = property.FindPropertyRelative( "m_x" );
            m_yProperty = property.FindPropertyRelative( "m_y" );
            m_zProperty = property.FindPropertyRelative( "m_z" );
            m_wProperty = property.FindPropertyRelative( "m_w" );
            m_objectProperty = property.FindPropertyRelative( "m_objectValue" );

            // Field
            m_valueTypeField = null;
            if ( m_mode == Mode.FullControl )
            {
                m_valueTypeField = new EnumField( ( AnyValue.ValueType )m_valueTypeProperty.enumValueIndex );
            }

            m_boolField = new Toggle() { value = AnyValueUtils.GetBoolFromProperty( m_xProperty ) };
            m_intField = new IntegerField() { value = AnyValueUtils.GetIntFromProperty( m_xProperty ) };
            m_floatField = new FloatField() { value = AnyValueUtils.GetFloatFromProperty( m_xProperty ) };
            m_stringField = new TextField() { value = m_stringProperty.stringValue };
            m_vector2Field = new Vector2Field() { value = AnyValueUtils.GetVector2FromProperties( m_xProperty, m_yProperty ) };
            m_vector3Field = new Vector3Field() { value = AnyValueUtils.GetVector3FromProperties( m_xProperty, m_yProperty, m_zProperty ) };
            m_vector4Field = new Vector4Field() { value = AnyValueUtils.GetVector4FromProperties( m_xProperty, m_yProperty, m_zProperty, m_wProperty ) };
            m_rectField = new RectField() { value = AnyValueUtils.GetRectFromProperties( m_xProperty, m_yProperty, m_zProperty, m_wProperty ) };
            m_colorField = new ColorField() { value = AnyValueUtils.GetColorFromProperties( m_xProperty, m_yProperty, m_zProperty, m_wProperty ) };
            m_objectField = new ObjectField() { allowSceneObjects = !property.IsIn<ScriptableObject>(), objectType = restrictedType, value = m_objectProperty.objectReferenceValue };
            HideFields();

            // Callback
            TrackProperties();
            RegisterHandlers();

            // uss class
            AddClasses();

            // layout
            AddToLayout();

            // StateMachine
            m_stateMachine = CreateStateMachine();
            m_stateMachine.ChangeState( ( AnyValue.ValueType )m_valueTypeProperty.enumValueIndex );
        }


        private void AddToLayout()
        {
            if ( m_mode == Mode.FullControl )
            {
                Add( m_valueTypeField );
            }

            Add( m_boolField );
            Add( m_intField );
            Add( m_floatField );
            Add( m_vector2Field );
            Add( m_vector3Field );
            Add( m_vector4Field );
            Add( m_rectField );
            Add( m_colorField );
            Add( m_objectField );
            Add( m_stringField );
        }


        private void AddClasses()
        {
            AddToClassList( ELEMENT_CLASS );
            if ( m_mode == Mode.FullControl )
            {
                m_valueTypeField.AddToClassList( TYPE_FIELD_CLASS );
            }

            m_boolField.AddToClassList( BOOL_CLASS );
            m_boolField.AddToClassList( VALUE_FIELD_CLASS );
            m_intField.AddToClassList( INT_CLASS );
            m_intField.AddToClassList( VALUE_FIELD_CLASS );
            m_floatField.AddToClassList( FLOAT_CLASS );
            m_floatField.AddToClassList( VALUE_FIELD_CLASS );
            m_vector2Field.AddToClassList( VECTOR2_CLASS );
            m_vector2Field.AddToClassList( VALUE_FIELD_CLASS );
            m_vector3Field.AddToClassList( VECTOR3_CLASS );
            m_vector3Field.AddToClassList( VALUE_FIELD_CLASS );
            m_vector4Field.AddToClassList( VECTOR4_CLASS );
            m_vector4Field.AddToClassList( VALUE_FIELD_CLASS );
            m_rectField.AddToClassList( RECT_CLASS );
            m_rectField.AddToClassList( VALUE_FIELD_CLASS );
            m_colorField.AddToClassList( COLOR_CLASS );
            m_colorField.AddToClassList( VALUE_FIELD_CLASS );
            m_stringField.AddToClassList( STRING_CLASS );
            m_stringField.AddToClassList( VALUE_FIELD_CLASS );
            m_objectField.AddToClassList( OBJECT_CLASS );
            m_objectField.AddToClassList( VALUE_FIELD_CLASS );

            style.flexDirection = FlexDirection.Row;
            style.alignContent = Align.Center;
        }


        private StateMachine<AnyValue.ValueType> CreateStateMachine()
        {
            return new Builder(
                new TypeState( AnyValue.ValueType.Bool, new VisualElement[1] { m_boolField } )
                ).AddStates(
                new List<State>( ( int )AnyValue.ValueType.Object ){
                    new TypeState( AnyValue.ValueType.String , new VisualElement[1] { m_stringField  } ),
                    new TypeState( AnyValue.ValueType.Int    , new VisualElement[1] { m_intField     } ),
                    new TypeState( AnyValue.ValueType.Float  , new VisualElement[1] { m_floatField   } ),
                    new TypeState( AnyValue.ValueType.Vector2, new VisualElement[1] { m_vector2Field } ),
                    new TypeState( AnyValue.ValueType.Vector3, new VisualElement[1] { m_vector3Field } ),
                    new TypeState( AnyValue.ValueType.Vector4, new VisualElement[1] { m_vector4Field } ),
                    new TypeState( AnyValue.ValueType.Rect   , new VisualElement[1] { m_rectField    } ),
                    new TypeState( AnyValue.ValueType.Color  , new VisualElement[1] { m_colorField   } ),
                    new TypeState( AnyValue.ValueType.Object , new VisualElement[1] { m_objectField } ),
                } ).Build();
        }


        private void RegisterHandlers()
        {
            if ( m_mode == Mode.FullControl )
            {
                m_valueTypeField.RegisterCallback<ChangeEvent<Enum>>( @event =>
                {
                    m_valueTypeProperty.enumValueIndex = ( int )( AnyValue.ValueType )@event.newValue;
                    ApplyAndUpdateProperties();
                    m_stateMachine.ChangeState( ( AnyValue.ValueType )@event.newValue );
                } );
            }

            m_boolField.RegisterCallback<ChangeEvent<bool>>( @event =>
            {
                AnyValueUtils.SetBoolToProperty( m_xProperty, @event.newValue );
                ApplyAndUpdateProperties();

            } );

            m_floatField.RegisterCallback<ChangeEvent<float>>( @event =>
            {
                AnyValueUtils.SetFloatToProperty( m_xProperty, @event.newValue );
                ApplyAndUpdateProperties();
            } );

            m_intField.RegisterCallback<ChangeEvent<int>>( @event =>
            {
                AnyValueUtils.SetIntToProperty( m_xProperty, @event.newValue );
                ApplyAndUpdateProperties();
            } );

            m_stringField.RegisterCallback<ChangeEvent<string>>( @event =>
            {
                AnyValueUtils.SetStringToProperty( m_stringProperty, @event.newValue );
                ApplyAndUpdateProperties();
            } );

            m_objectField.RegisterCallback<ChangeEvent<UnityEngine.Object>>( @event =>
            {
                AnyValueUtils.SetObjectToProperty( m_objectProperty, @event.newValue );
                ApplyAndUpdateProperties();
            } );

            m_vector2Field.RegisterCallback<ChangeEvent<Vector2>>( @event =>
            {
                AnyValueUtils.SetVector2ToProperties( m_xProperty, m_yProperty, @event.newValue );
                ApplyAndUpdateProperties();
            } );

            m_vector3Field.RegisterCallback<ChangeEvent<Vector3>>( @event =>
            {
                AnyValueUtils.SetVector3ToProperties( m_xProperty, m_yProperty, m_zProperty, @event.newValue );
                ApplyAndUpdateProperties();
            } );

            m_vector4Field.RegisterCallback<ChangeEvent<Vector4>>( @event =>
            {
                AnyValueUtils.SetVector4ToProperties( m_xProperty, m_yProperty, m_zProperty, m_wProperty, @event.newValue );
                ApplyAndUpdateProperties();
            } );

            m_rectField.RegisterCallback<ChangeEvent<Rect>>( @event =>
            {
                AnyValueUtils.SetRectToProperties( m_xProperty, m_yProperty, m_zProperty, m_wProperty, @event.newValue );
                ApplyAndUpdateProperties();
            } );

            m_colorField.RegisterCallback<ChangeEvent<Color>>( @event =>
            {
                AnyValueUtils.SetColorToProperties( m_xProperty, m_yProperty, m_zProperty, m_wProperty, @event.newValue );
                ApplyAndUpdateProperties();
            } );
        }

        private void ApplyAndUpdateProperties()
        {
            m_xProperty.serializedObject.ApplyModifiedProperties();
            m_xProperty.serializedObject.Update();
        }


        private void TrackProperties()
        {
            this.TrackPropertyValue( m_valueTypeProperty, property =>
            {
                m_stateMachine.ChangeState( ( AnyValue.ValueType )property.enumValueIndex );
                if ( m_mode == Mode.FullControl )
                {
                    m_valueTypeField.SetValueWithoutNotify( ( AnyValue.ValueType )property.enumValueIndex );
                }
            } );

            this.TrackPropertyValue( m_xProperty, property =>
            {
                m_boolField.SetValueWithoutNotify( AnyValueUtils.GetBoolFromProperty( property ) );
                m_intField.SetValueWithoutNotify( AnyValueUtils.GetIntFromProperty( property ) );
                m_floatField.SetValueWithoutNotify( AnyValueUtils.GetFloatFromProperty( property ) );
                m_vector2Field.SetValueWithoutNotify( AnyValueUtils.GetVector2FromProperties( property, m_yProperty ) );
                m_vector3Field.SetValueWithoutNotify( AnyValueUtils.GetVector3FromProperties( property, m_yProperty, m_zProperty ) );
                m_vector4Field.SetValueWithoutNotify( AnyValueUtils.GetVector4FromProperties( property, m_yProperty, m_zProperty, m_wProperty ) );
                m_rectField.SetValueWithoutNotify( AnyValueUtils.GetRectFromProperties( property, m_yProperty, m_zProperty, m_wProperty ) );
                m_colorField.SetValueWithoutNotify( AnyValueUtils.GetColorFromProperties( property, m_yProperty, m_zProperty, m_wProperty ) );
            } );

            this.TrackPropertyValue( m_yProperty, property =>
            {
                m_vector2Field.SetValueWithoutNotify( AnyValueUtils.GetVector2FromProperties( m_xProperty, property ) );
                m_vector3Field.SetValueWithoutNotify( AnyValueUtils.GetVector3FromProperties( m_xProperty, property, m_zProperty ) );
                m_vector4Field.SetValueWithoutNotify( AnyValueUtils.GetVector4FromProperties( m_xProperty, property, m_zProperty, m_wProperty ) );
                m_rectField.SetValueWithoutNotify( AnyValueUtils.GetRectFromProperties( m_xProperty, property, m_zProperty, m_wProperty ) );
                m_colorField.SetValueWithoutNotify( AnyValueUtils.GetColorFromProperties( m_xProperty, property, m_zProperty, m_wProperty ) );
            } );

            this.TrackPropertyValue( m_zProperty, property =>
            {
                m_vector3Field.SetValueWithoutNotify( AnyValueUtils.GetVector3FromProperties( m_xProperty, m_yProperty, property ) );
                m_vector4Field.SetValueWithoutNotify( AnyValueUtils.GetVector4FromProperties( m_xProperty, m_yProperty, property, m_wProperty ) );
                m_rectField.SetValueWithoutNotify( AnyValueUtils.GetRectFromProperties( m_xProperty, m_yProperty, property, m_wProperty ) );
                m_colorField.SetValueWithoutNotify( AnyValueUtils.GetColorFromProperties( m_xProperty, m_yProperty, property, m_wProperty ) );
            } );

            this.TrackPropertyValue( m_wProperty, property =>
            {
                m_vector4Field.SetValueWithoutNotify( AnyValueUtils.GetVector4FromProperties( m_xProperty, m_yProperty, m_zProperty, property ) );
                m_rectField.SetValueWithoutNotify( AnyValueUtils.GetRectFromProperties( m_xProperty, m_yProperty, m_zProperty, property ) );
                m_colorField.SetValueWithoutNotify( AnyValueUtils.GetColorFromProperties( m_xProperty, m_yProperty, m_zProperty, property ) );
            } );

            this.TrackPropertyValue( m_stringProperty, property =>
            {
                m_stringField.SetValueWithoutNotify( property.stringValue );
            } );

            this.TrackPropertyValue( m_objectProperty, property =>
            {
                m_objectField.SetValueWithoutNotify( property.objectReferenceValue );
            } );
        }


        private void HideFields()
        {
            m_boolField.Hide();
            m_intField.Hide();
            m_floatField.Hide();
            m_stringField.Hide();
            m_vector2Field.Hide();
            m_vector3Field.Hide();
            m_vector4Field.Hide();
            m_rectField.Hide();
            m_colorField.Hide();
            m_objectField.Hide();
        }
    }
}
