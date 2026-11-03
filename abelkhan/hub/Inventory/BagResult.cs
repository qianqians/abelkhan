namespace hub.inventory;

/// <summary>
/// 背包操作结果。预期内的失败（背包满、数量不足等）通过返回值表达，不抛异常；
/// 只有调用方传了 null 等编程错误才由实现抛异常。
/// </summary>
/// <param name="Ok">是否成功。</param>
/// <param name="Error">失败原因。</param>
/// <param name="Affected">本次实际变动的物品数量（添加 / 消费 / 删除）。</param>
/// <param name="Message">可直接返回给客户端或写日志的说明。</param>
public readonly record struct BagResult(bool Ok, BagError Error, long Affected, string Message)
{
    public bool Failed => !Ok;

    public static BagResult Success(long affected = 0) =>
        new(true, BagError.None, affected, string.Empty);

    public static BagResult Fail(BagError error, string message) =>
        new(false, error, 0, message);
}
