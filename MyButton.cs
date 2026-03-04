using Godot;
using System;

public partial class MyButton : Button
{
    [Export]
    public Button TargetButton { get; set; }

    [Export]
    public Control TargetElement1 { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void OnButtonPressed()
    {
        //GetTree().ChangeScene("res://Scenes/Scene2.tscn");
        if (TargetButton != null)
            TargetButton.Text = TargetButton.Text + "+";

        if (TargetElement1.Position.X == 0)
            TargetElement1.Position = new(100, 100);
        else
            TargetElement1.Position = new(0, 0);

    }
}
