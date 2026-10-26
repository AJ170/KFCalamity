namespace Calamity.Input
{
	public class StickOrVirtualStickWrapper : HybridInputWrapper
	{
		public StickOrVirtualStickWrapper(string horizontalAxis, string verticalAxis, string virtualStickObjectName, bool invertHorizontal, bool invertVertical)
			: base(new StickInputWrapper(horizontalAxis, verticalAxis, invertHorizontal, invertVertical), new VirtualStickWrapper(virtualStickObjectName))
		{
		}
	}
}
