namespace TipExpert.Core.Invitation
{
    public interface IMailInvitationService
    {
        void SendInvitation(InvitedPlayer player);
    }
}