namespace TipExpert.Core.Invitation
{
    public interface IInvitationTokeService
    {
        string GetNewInvitaionToken();

        void UpdatePlayerForToken(string token);
    }
}