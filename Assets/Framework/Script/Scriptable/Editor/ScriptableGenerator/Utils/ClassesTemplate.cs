using System;

namespace Framework.Scriptable.Editor
{
    public static class Templates
    {
        public static VariableTemplate Variable = new VariableTemplate();
        public static VariableInjectorTemplate VariableInjector = new VariableInjectorTemplate();
        public static VariableReferenceTemplate VariableReference = new VariableReferenceTemplate();

        public static EventTemplate Event = new EventTemplate();
        public static EventInjectorTemplate EventInjector = new EventInjectorTemplate();
        public static EventReferenceTemplate EventReference = new EventReferenceTemplate();
    }


    // since we are modifying assembly and type might not exist yet we need a way to have those before assembly reload
    public class TypeData 
    {
        public string Name { get; private set; }
        public string Namespace {  get; private set; }

        public TypeData( string name, string @namespace  )
        {
            Name = name;
            Namespace = @namespace;
        }


        public TypeData( Type type ) : this( type.Name, type.Namespace ) {}


        public string GetSafeName()
        {
            return string.IsNullOrEmpty( Namespace ) ? Name : $"{Namespace}.{Name}";
        }


        public static explicit operator TypeData( Type type )
        {
            return new TypeData( type );
        }
    }


    // Only way I found to prevent even more duplication code, used as compile time string 
    // mimic c++ template<string value> with c# generic
    public interface IScriptableElement {}
    namespace Tag
    {
        public class Event : IScriptableElement {}
        public class Variable : IScriptableElement {}
    }


    public class VariableTemplate : ScriptableTemplate<Tag.Variable> {}
    public class VariableInjectorTemplate : ScriptableInjectorTemplate<Tag.Variable> {}
    public class VariableReferenceTemplate : ScriptableReferenceTemplate<Tag.Variable> {}
    public class EventTemplate : ScriptableTemplate<Tag.Event> {}
    public class EventInjectorTemplate : ScriptableInjectorTemplate<Tag.Event> {}
    public class EventReferenceTemplate : ScriptableReferenceTemplate<Tag.Event> {}


    public class ScriptableTemplate<T> where T : IScriptableElement
    {
        public string GetClassName( TypeData type )
        {
            return $"{type.Name}{typeof( T ).Name}";
        }


        public string GenerateClassContent( TypeData type, string @namespace, string category )
        {
            return
@$"
using UnityEngine;  

namespace {@namespace}
{{
    [CreateAssetMenu( fileName = ""{GetClassName( type )}"", menuName = ""Scriptable/{typeof( T ).Name}/{category}/{type.Name}"" )]
    public class {GetClassName( type )} : Framework.Scriptable.Scriptable{typeof( T ).Name}<{type.GetSafeName()}> {{}}
}}
";
        }
    }


    public class ScriptableInjectorTemplate<T> where T : IScriptableElement
    {
        public string GetClassName( TypeData type )
        {
            return $"{type.Name}{typeof( T ).Name}Injector";
        }


        public string GenerateClassContent( TypeData type, TypeData scriptableType, string @namespace )
        {
            return
@$"
using UnityEngine;  

namespace {@namespace}
{{
    public class {GetClassName( type )} : Framework.Scriptable.Runtime{typeof( T ).Name}Injector<{type.GetSafeName()}, {scriptableType.GetSafeName()}> {{}}
}}
";
        }
    }


    public class ScriptableReferenceTemplate<T> where T : IScriptableElement
    {
        public string GetClassName( TypeData type )
        {
            return $"{type.Name}{typeof( T ).Name}Reference";
        }


        public string GenerateClassContent( TypeData type, TypeData scriptableType, TypeData injectorType, string @namespace )
        {
            return
@$"
using UnityEngine;  

namespace {@namespace}
{{
    [System.Serializable]
    public class {GetClassName( type )} : Framework.Scriptable.Scriptable{typeof( T ).Name}Reference<{type.GetSafeName()}, {scriptableType.GetSafeName()}, {injectorType.GetSafeName()}> {{}}
}}
";
        }
    }
}
