using System;

[Flags]
public enum TKSwipeDirection
{
	Left = 1,
	Right = 2,
	Up = 4,
	Down = 0x10,
	Horizontal = 3,
	Vertical = 0x14,
	All = 0x17
}
