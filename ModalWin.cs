using Godot;
using System;

public partial class ModalWin : Control
{
	[Signal]
	public delegate void CloseWinModalEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnCloseButtonPressed()
	{
		EmitSignal(SignalName.CloseWinModal);
	}

	public void SetModalTextsandScore(string title, string scoreText)
	{
		GetNode<Label>("ColorRect/TextureRect/VBoxContainer/Title").Text = title;
		GetNode<Label>("ColorRect/TextureRect/VBoxContainer/ScoreText").Text = scoreText;
	}
}
