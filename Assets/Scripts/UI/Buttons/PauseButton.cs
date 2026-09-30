public class PauseButton : UIButton<PauseButton.PauseButtonType>
{
    public enum PauseButtonType
    {
        RESUME
    }

    private void ResumeGame()
    {
        GameManager.s_Instance.ResumeGame();
    }


    public override void Interact()
    {
        base.Interact();

        switch (ButtonType)
        {
            case PauseButtonType.RESUME:
                {
                    ResumeGame();
                }
                break;
            default:
                {
                }
                break;
        }
    }
}
