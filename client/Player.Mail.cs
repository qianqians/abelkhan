using consts;
using engine;

namespace client;

/// <summary>
/// 邮件协议（拉取可领取的全局邮件 / 领取指定邮件）。
/// </summary>
public abstract partial class Player
{
    /// <summary>
    /// 请求领取邮件：mailId 为空 = 拉取当前可领取的全局邮件列表；
    /// mailId 非空 = 领取指定邮件（hub 会标记为已领取）。
    /// </summary>
    public Task<Result<ClientRequestMailResult, string>> RequestMail(string mailId = "")
    {
        return Request<ClientRequestMail, ClientRequestMailResult>(
            Consts.ClientRequestMail,
            new ClientRequestMail { MailId = mailId });
    }
}
