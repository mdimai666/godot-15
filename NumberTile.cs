using Godot;
using System;

public partial class NumberTile : Control
{
	public Action<InputEventMouseButton> OnClick;

	public int PositionIndex { get; set; } = -1;

	public override void _Ready()
	{
		//SetLayoutMode(LayoutModeEnum.Absolute);
		GuiInput += OnGuiInput;
	}

	private void OnGuiInput(InputEvent @event)
	{
		// Проверяем, является ли событие нажатием кнопки мыши
		if (@event is InputEventMouseButton mouseEvent)
		{
			// Проверяем, что это левая кнопка (Left) и она нажата (Pressed)
			if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
			{
				//GD.Print("Клик по NinePatchRect!");
				OnClick.Invoke(mouseEvent);
			}
		}
	}
}
