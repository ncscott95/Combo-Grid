using UnityEngine;

public class PlayerController2D : PlayerControllerBase
{
    public override void OnEnable()
    {
        base.OnEnable();

        Actions.Player.Jump.performed += ctx => Jump();
    }

    public override void OnDisable()
    {
        base.OnDisable();

        Actions.Player.Jump.performed -= ctx => Jump();
    }

    public override void Update()
    {
        
    }

    public override void Attack()
    {
        // Implement 2D attack logic here
    }

    public override void Dodge()
    {
        // Implement 2D dodge logic here
    }

    public void Jump()
    {
        // Implement 2D jump logic here
    }
}
