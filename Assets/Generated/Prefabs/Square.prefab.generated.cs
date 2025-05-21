// this file is auto-generated, all manual edit will be lost on the next generation
// if you want to edit those function, do so through the inspector of the matching action
namespace Generated
{
	public static class SquareGameplayAction
	{
		public static void Action6bd23e2bef594ee5846feb2829079642( object[] args )
		{
			
		}
		
		
		public static void Actioncecfbf0f82264c8e8009f4bf5ea8a884( object[] args )
		{
			bool var0 = Framework.Core.CoreUtils.Not( ( bool )args[0] );
			Framework.Core.CoreUtils.Behaviour_Enable( ( UnityEngine.Behaviour )( ( Framework.Core.AnyValue )( ( Framework.Core.ModifierArgs )args[4 + 1] ).Args[0] ).Get<UnityEngine.Object>(), ( bool )var0 );
			
		}
		
		
		public static void Actione4cb8dd4948a4e63b203010ff77dec14( object[] args )
		{
			
		}
		
		
		[UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSplashScreen)]
		public static void Register()
		{
			Framework.Core.ActyxRegistry.Register( new Framework.Core.ID( "6bd23e2b-ef59-4ee5-846f-eb2829079642" ), Action6bd23e2bef594ee5846feb2829079642 );
			Framework.Core.ActyxRegistry.Register( new Framework.Core.ID( "cecfbf0f-8226-4c8e-8009-f4bf5ea8a884" ), Actioncecfbf0f82264c8e8009f4bf5ea8a884 );
			Framework.Core.ActyxRegistry.Register( new Framework.Core.ID( "e4cb8dd4-948a-4e63-b203-010ff77dec14" ), Actione4cb8dd4948a4e63b203010ff77dec14 );
			
		}
		
	}
	
}
