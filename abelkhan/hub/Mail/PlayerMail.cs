using consts;
using core;
using engine;
using hub.inventory;

namespace hub;

/// <summary>
/// 玩家实体的邮件协议（领取全局邮件 + 附件入包）与协议条目转换。
/// 邮件数据本身在 <see cref="GlobalMailBox"/>（Redis），领取条件由 <see cref="Service.GetMailCondition"/> 提供。
/// </summary>
public abstract partial class Player
{
    /// <summary>
    /// 把邮件附件放进背包。<see cref="IBag.AddRange"/> 是原子的：要么全部放入，要么背包保持原样。
    /// 数量 &lt;= 0 的附件会被跳过。
    /// </summary>
    protected BagResult GrantMailAttachments(GlobalMail mail)
    {
        var items = new List<IItem>(mail.Attachments.Count);
        foreach (var attachment in mail.Attachments)
        {
            if (attachment.Count <= 0)
            {
                continue;
            }

            IItem? item;
            try
            {
                item = CreateItem(attachment.ItemId, attachment.Count);
            }
            catch (Exception e)
            {
                return BagResult.Fail(BagError.InvalidArgument,
                    $"邮件附件 {attachment.ItemId} 配置非法: {e.Message}");
            }

            if (item == null)
            {
                return BagResult.Fail(BagError.UnknownItem,
                    $"邮件附件 {attachment.ItemId} 无法创建");
            }

            items.Add(item);
        }

        return items.Count == 0 ? BagResult.Success() : Bag.AddRange(items);
    }

    /// <summary>
    /// 注册「客户端请求领取邮件」协议：
    ///   mail_id 为空  -> 返回当前可领取的全局邮件（不标记领取）
    ///   mail_id 非空  -> 领取指定邮件（标记已领取、附件原子放入背包并返回）
    /// 条件（等级 / 建角天数）由游戏的 Service.GetMailCondition 提供。
    /// 附件发放失败（背包满 / 配置错误）时会回滚已领取标记，避免玩家丢邮件。
    /// </summary>
    internal void RegisterMailRequest(Service service)
    {
        RegisterRequest<ClientRequestMail, ClientRequestMailResult>(Consts.ClientRequestMail, async req =>
        {
            var result = new ClientRequestMailResult();
            var box = service.GlobalMailBox;
            if (box == null)
            {
                return Result<ClientRequestMailResult, string>.Ok(result);
            }

            var cond = service.GetMailCondition(UserId);
            if (cond == null)
            {
                return Result<ClientRequestMailResult, string>.Ok(result);
            }

            if (string.IsNullOrEmpty(req.MailId))
            {
                var mails = await box.GetUnclaimed(UserId, cond.Value.Level, cond.Value.CreateDays);
                foreach (var mail in mails)
                {
                    result.Mails.Add(ToMailInfo(mail));
                }
            }
            else
            {
                var mail = await box.GetClaimable(UserId, cond.Value.Level, cond.Value.CreateDays, req.MailId);
                if (mail == null)
                {
                    return Result<ClientRequestMailResult, string>.Err("mail not claimable");
                }

                // SADD 原子判定：并发/重复领取时只有第一次成功
                var claimedNow = await box.MarkClaimed(UserId, req.MailId);
                if (!claimedNow)
                {
                    return Result<ClientRequestMailResult, string>.Err("mail already claimed");
                }

                // 附件发放失败（背包满 / 附件配置错误）时回滚认领标记，
                // 否则玩家会因为一次失败的领取永久丢掉这封邮件。
                BagResult grant = default;
                try
                {
                    grant = GrantMailAttachments(mail);
                }
                catch (Exception e)
                {
                    await box.UnmarkClaimed(UserId, req.MailId);
                    Log.Error("claim mail:{0} user:{1} grant items failed! {2}", req.MailId, UserId, e);
                    return Result<ClientRequestMailResult, string>.Err("mail claim failed");
                }

                if (grant.Failed)
                {
                    await box.UnmarkClaimed(UserId, req.MailId);
                    return Result<ClientRequestMailResult, string>.Err($"mail claim failed: {grant.Message}");
                }

                result.Mails.Add(ToMailInfo(mail));
            }

            return Result<ClientRequestMailResult, string>.Ok(result);
        });
    }

    private static MailInfo ToMailInfo(GlobalMail mail)
    {
        var info = new MailInfo
        {
            MailId = mail.MailId,
            Title = mail.Title,
            Content = mail.Content,
            Sender = mail.Sender,
            CreateTime = (ulong)mail.CreateTime,
            PopUp = mail.PopUp,
        };

        foreach (var attachment in mail.Attachments)
        {
            info.Attachments.Add(new MailAttachment
            {
                ItemId = attachment.ItemId,
                Count = (uint)Math.Max(0, attachment.Count),
            });
        }

        return info;
    }
}
