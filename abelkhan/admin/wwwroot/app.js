'use strict';

const $ = (id) => document.getElementById(id);

const state = {
    players: { page: 1, pageSize: 20 },
    payments: { page: 1, pageSize: 20 },
    gm: { page: 1, pageSize: 20 },
    mail: { page: 1, pageSize: 20 },
    shopProducts: { page: 1, pageSize: 20, total: 0 },
    shopListings: { page: 1, pageSize: 20, total: 0 },
    shopOrders: { page: 1, pageSize: 20, total: 0 },
};

let editingShopProductId = null;

let overviewTimer = null;

// ---------------- 基础工具 ----------------

async function request(path, options = {}, retried = false) {
    const token = localStorage.getItem('adminToken');
    const headers = Object.assign({}, options.headers || {});
    if (token) headers['Authorization'] = 'Bearer ' + token;

    const resp = await fetch(path, Object.assign({}, options, { headers }));
    if (resp.status === 401 && !retried) {
        const t = prompt('该接口需要访问 Token，请输入：');
        if (t) {
            localStorage.setItem('adminToken', t);
            return request(path, options, true);
        }
        throw new Error('unauthorized');
    }
    const body = await resp.json().catch(() => ({}));
    if (!resp.ok) {
        throw new Error(body.error || ('HTTP ' + resp.status));
    }
    return body;
}

function postJson(path, data) {
    return request(path, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
    });
}

function putJson(path, data) {
    return request(path, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
    });
}

function setConn(ok, text) {
    $('conn-status').classList.toggle('ok', ok);
    $('conn-text').textContent = text;
}

function notify(msg, isError = false) {
    let el = $('toast');
    if (!el) {
        el = document.createElement('div');
        el.id = 'toast';
        el.style.cssText =
            'position:fixed;bottom:24px;right:24px;z-index:200;' +
            'background:#1e293b;border:1px solid #334155;border-radius:10px;' +
            'padding:12px 18px;color:#e2e8f0;box-shadow:0 8px 30px rgba(0,0,0,.4);';
        document.body.appendChild(el);
    }
    el.style.borderColor = isError ? '#f87171' : '#34d399';
    el.textContent = msg;
    clearTimeout(el._t);
    el._t = setTimeout(() => el.remove(), 2600);
}

function fmt(v) {
    const n = Number(v);
    if (!isFinite(n)) return String(v ?? '');
    return n.toLocaleString('zh-CN');
}

function fmtTime(v) {
    if (v == null || v === '') return '';
    const n = Number(v);
    if (!isFinite(n) || n === 0) return String(v);
    const ms = n < 1e12 ? n * 1000 : n;
    return new Date(ms).toLocaleString('zh-CN', { hour12: false });
}

function firstOf(doc, keys) {
    for (const k of keys) {
        const v = doc[k];
        if (v != null && v !== '') return v;
    }
    return '';
}

function cell(text, cls) {
    const td = document.createElement('td');
    td.textContent = text == null ? '' : String(text);
    if (cls) td.className = cls;
    return td;
}

function emptyRow(colspan) {
    const tr = document.createElement('tr');
    const td = document.createElement('td');
    td.colSpan = colspan;
    td.className = 'empty';
    td.textContent = '暂无数据';
    tr.appendChild(td);
    return tr;
}

function renderPager(infoId, prevId, nextId, section, loadFn) {
    const s = state[section];
    $(infoId).textContent = `第 ${s.page} 页（共 ${s.total} 条）`;
    $(prevId).disabled = s.page <= 1;
    $(nextId).disabled = s.page * s.pageSize >= s.total;
    $(prevId).onclick = () => { if (s.page > 1) { s.page--; loadFn(); } };
    $(nextId).onclick = () => { if (s.page * s.pageSize < s.total) { s.page++; loadFn(); } };
}

// ---------------- 监控大盘 ----------------

async function loadOverview() {
    try {
        const data = await request('/api/overview');
        setConn(true, '已连接');
        $('ov-online-players').textContent = fmt(data.onlinePlayers);
        $('ov-online-connections').textContent = fmt(data.onlineConnections);
        $('ov-total-players').textContent = fmt(data.totalPlayers);
        $('ov-today-registers').textContent = fmt(data.todayRegisters);
        $('ov-yesterday-registers').textContent = fmt(data.yesterdayRegisters);
        $('ov-today-recharge').textContent = fmt(data.todayRecharge);
        $('ov-yesterday-recharge').textContent = fmt(data.yesterdayRecharge);
        renderChart(data);
    } catch (e) {
        setConn(false, '连接失败');
        console.error(e);
    }
}

function renderChart(data) {
    const el = $('overview-chart');
    el.innerHTML = '';
    const groups = [
        { caption: '注册', recharge: false, items: [['今日', data.todayRegisters], ['昨日', data.yesterdayRegisters]] },
        { caption: '充值', recharge: true, items: [['今日', data.todayRecharge], ['昨日', data.yesterdayRecharge]] },
    ];
    const all = groups.flatMap((g) => g.items.map((i) => Number(i[1]) || 0));
    const max = Math.max(1, ...all);

    for (const g of groups) {
        const group = document.createElement('div');
        group.className = 'chart-group';
        const bars = document.createElement('div');
        bars.className = 'chart-bars';
        for (const [label, val] of g.items) {
            const bar = document.createElement('div');
            bar.className = 'chart-bar' + (g.recharge ? ' recharge' : '');
            bar.style.height = Math.max(2, Math.round((Number(val) || 0) / max * 100)) + '%';
            const span = document.createElement('span');
            span.textContent = label + ' ' + fmt(val);
            bar.appendChild(span);
            bars.appendChild(bar);
        }
        const caption = document.createElement('div');
        caption.className = 'chart-caption';
        caption.textContent = g.caption;
        group.appendChild(bars);
        group.appendChild(caption);
        el.appendChild(group);
    }
}

// ---------------- 玩家查询 ----------------

async function loadPlayers() {
    const s = state.players;
    const keyword = $('player-keyword').value.trim();
    try {
        const q = new URLSearchParams({ page: s.page, pageSize: s.pageSize });
        if (keyword) q.set('keyword', keyword);
        const data = await request('/api/players?' + q.toString());
        s.total = data.total;
        const tbody = $('player-table-body');
        tbody.innerHTML = '';
        if (!data.items || !data.items.length) {
            tbody.appendChild(emptyRow(4));
        } else {
            for (const d of data.items) {
                const tr = document.createElement('tr');
                tr.appendChild(cell(firstOf(d, ['EntityId', 'entity_id', '_id']), 'mono'));
                tr.appendChild(cell(firstOf(d, ['EntityType', 'entity_type']), 'dim'));
                tr.appendChild(cell(firstOf(d, ['UserId', 'user_id']), 'mono'));
                const actTd = document.createElement('td');
                const btn = document.createElement('button');
                btn.className = 'btn';
                btn.textContent = '详情';
                btn.onclick = () => showPlayerDetail(d);
                actTd.appendChild(btn);
                tr.appendChild(actTd);
                tbody.appendChild(tr);
            }
        }
        renderPager('player-page-info', 'player-prev', 'player-next', 'players', loadPlayers);
    } catch (e) {
        notify('查询失败：' + e.message, true);
    }
}

function showPlayerDetail(d) {
    $('player-detail-body').textContent = JSON.stringify(d, null, 2);
    $('player-detail').classList.remove('hidden');
}

// ---------------- 支付查询 ----------------

async function loadPayments() {
    const s = state.payments;
    try {
        const q = new URLSearchParams({ page: s.page, pageSize: s.pageSize });
        const userId = $('pay-user-id').value.trim();
        const start = $('pay-start').value;
        const end = $('pay-end').value;
        if (userId) q.set('userId', userId);
        if (start) q.set('start', start);
        if (end) q.set('end', end);
        const data = await request('/api/payments?' + q.toString());
        s.total = data.total;
        const tbody = $('pay-table-body');
        tbody.innerHTML = '';
        if (!data.items || !data.items.length) {
            tbody.appendChild(emptyRow(4));
        } else {
            for (const d of data.items) {
                const tr = document.createElement('tr');
                tr.appendChild(cell(firstOf(d, ['_id', 'OrderId', 'order_id']), 'mono'));
                tr.appendChild(cell(firstOf(d, ['UserId', 'user_id']), 'mono'));
                tr.appendChild(cell(fmt(firstOf(d, ['Amount', 'amount', 'PayAmount', 'Money', 'money']))));
                tr.appendChild(cell(fmtTime(firstOf(d, ['PayTime', 'pay_time', 'CreateTime', 'create_time'])), 'dim'));
                tbody.appendChild(tr);
            }
        }
        renderPager('pay-page-info', 'pay-prev', 'pay-next', 'payments', loadPayments);
    } catch (e) {
        notify('查询失败：' + e.message, true);
    }
}

// ---------------- GM 面板 ----------------

async function loadGmLogs() {
    const s = state.gm;
    try {
        const q = new URLSearchParams({ page: s.page, pageSize: s.pageSize });
        const data = await request('/api/gm/logs?' + q.toString());
        s.total = data.total;
        const tbody = $('gm-table-body');
        tbody.innerHTML = '';
        if (!data.items || !data.items.length) {
            tbody.appendChild(emptyRow(5));
        } else {
            for (const d of data.items) {
                const tr = document.createElement('tr');
                tr.appendChild(cell(firstOf(d, ['Command', 'command']), 'mono'));
                tr.appendChild(cell(firstOf(d, ['PlayerId', 'player_id']), 'mono'));
                tr.appendChild(cell(firstOf(d, ['Args', 'args']), 'dim'));
                tr.appendChild(cell(firstOf(d, ['Result', 'result']), 'dim'));
                tr.appendChild(cell(fmtTime(firstOf(d, ['CreateTime', 'create_time'])), 'dim'));
                tbody.appendChild(tr);
            }
        }
        renderPager('gm-page-info', 'gm-prev', 'gm-next', 'gm', loadGmLogs);
    } catch (e) {
        notify('查询失败：' + e.message, true);
    }
}

async function submitGm() {
    const command = $('gm-command').value.trim();
    if (!command) { notify('请填写命令', true); return; }
    try {
        const res = await postJson('/api/gm/command', {
            command,
            playerId: $('gm-player-id').value.trim(),
            args: $('gm-args').value.trim(),
        });
        notify(res.mode === 'local' ? ('本地执行：' + (res.result || 'ok')) : ('已下发到玩家 ' + res.playerId));
        $('gm-command').value = '';
        $('gm-args').value = '';
        state.gm.page = 1;
        loadGmLogs();
    } catch (e) {
        notify('下发失败：' + e.message, true);
    }
}

// ---------------- 邮件发送 ----------------

async function loadMail() {
    const s = state.mail;
    try {
        const q = new URLSearchParams({ page: s.page, pageSize: s.pageSize });
        const data = await request('/api/mail?' + q.toString());
        s.total = data.total;
        const tbody = $('mail-table-body');
        tbody.innerHTML = '';
        if (!data.items || !data.items.length) {
            tbody.appendChild(emptyRow(5));
        } else {
            for (const d of data.items) {
                const tr = document.createElement('tr');
                tr.appendChild(cell(mailRecipients(d), 'mono'));
                tr.appendChild(cell(firstOf(d, ['Sender', 'sender']), 'dim'));
                tr.appendChild(cell(firstOf(d, ['Title', 'title'])));
                tr.appendChild(cell(firstOf(d, ['Content', 'content']), 'dim'));
                tr.appendChild(cell(fmtTime(firstOf(d, ['CreateTime', 'create_time'])), 'dim'));
                tbody.appendChild(tr);
            }
        }
        renderPager('mail-page-info', 'mail-prev', 'mail-next', 'mail', loadMail);
    } catch (e) {
        notify('查询失败：' + e.message, true);
    }
}

async function submitMail() {
    const title = $('mail-title').value.trim();
    const content = $('mail-content').value.trim();
    if (!title || !content) { notify('请填写标题和正文', true); return; }
    const sender = $('mail-sender').value.trim();
    const attachments = parseAttachments($('mail-attachments').value);
    const playerIds = $('mail-user-id').value
        .split(/[\s,，;；]+/)
        .map((s) => s.trim())
        .filter(Boolean);
    try {
        const res = await postJson('/api/mail/send', {
            title,
            content,
            sender,
            attachments,
            playerIds,
            expireSeconds: parseInt($('mail-expire').value || '0', 10),
            minLevel: parseInt($('mail-min-level').value || '0', 10),
            maxLevel: parseInt($('mail-max-level').value || '0', 10),
            minCreateDays: parseInt($('mail-min-create-days').value || '0', 10),
            maxCreateDays: parseInt($('mail-max-create-days').value || '0', 10),
            validDays: parseInt($('mail-valid-days').value || '0', 10),
            popUp: $('mail-popup').checked,
        });
        notify(res.global ? '全局邮件已发送' : ('已发送给 ' + res.count + ' 名玩家'));
        $('mail-title').value = '';
        $('mail-content').value = '';
        $('mail-attachments').value = '';
        state.mail.page = 1;
        loadMail();
    } catch (e) {
        notify('发送失败：' + e.message, true);
    }
}

function parseAttachments(s) {
    if (!s) return [];
    return s.split(/[\s,，;；]+/)
        .filter(Boolean)
        .map((pair) => {
            const idx = pair.indexOf(':');
            if (idx < 0) return { itemId: pair.trim(), count: 1 };
            return {
                itemId: pair.slice(0, idx).trim(),
                count: parseInt(pair.slice(idx + 1), 10) || 1,
            };
        })
        .filter((a) => a.itemId);
}

function mailRecipients(d) {
    const ids = d.PlayerIds || d.player_ids || d.playerIds;
    if (Array.isArray(ids)) {
        if (ids.length === 0) return '全体';
        return ids.slice(0, 5).join(', ') + (ids.length > 5 ? ` 等${ids.length}人` : '');
    }
    if (ids == null || ids === '') return '全体';
    return String(ids);
}

// ---------------- 商城 ----------------

const SHOP_STATUS_TEXT = {
    active: '在售',
    pending: '未开始',
    sold: '已成交',
    expired: '已过期',
    cancelled: '已取消',
};

const SHOP_ORDER_TYPE_TEXT = {
    buy: '一口价',
    auction: '拍卖成交',
    expire: '流拍退回',
    cancel: '取消',
};

function shopStatusText(status) {
    return SHOP_STATUS_TEXT[status] || status || '';
}

function shopModeText(mode) {
    return Number(mode) === 2 ? '限时拍卖' : '一口价';
}

function msToLocalInput(ms) {
    const n = Number(ms) || 0;
    if (!n) return '';
    const d = new Date(n);
    const pad = (x) => String(x).padStart(2, '0');
    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
}

function localInputToMs(value) {
    if (!value) return 0;
    const t = Date.parse(value);
    return Number.isFinite(t) ? t : 0;
}

function rewardsToText(items) {
    if (!Array.isArray(items)) return '';
    return items
        .map((i) => `${firstOf(i, ['ItemId', 'itemId'])}:${i.Count != null ? i.Count : i.count}`)
        .join(',');
}

function actionButton(text, onClick) {
    const btn = document.createElement('button');
    btn.className = 'btn';
    btn.textContent = text;
    btn.onclick = onClick;
    return btn;
}

async function loadShopMeta() {
    try {
        const m = await request('/api/shop/meta');
        $('shop-meta-info').textContent =
            `默认货币 ${m.currencyItemId} · 寄拍税 ${m.taxPercent}% · 每人最多寄拍 ${m.maxListingsPerPlayer} 件`;
    } catch (e) {
        $('shop-meta-info').textContent = '';
    }
}

async function loadShop() {
    await loadShopMeta();
    await loadShopProducts();
    await loadShopListings();
    await loadShopOrders();
}

// ---------- 商品配置 ----------

async function loadShopProducts() {
    const s = state.shopProducts;
    try {
        const q = new URLSearchParams({ page: s.page, pageSize: s.pageSize });
        const keyword = $('shop-keyword').value.trim();
        const status = $('shop-status-filter').value;
        if (keyword) q.set('keyword', keyword);
        if (status) q.set('status', status);

        const data = await request('/api/shop/products?' + q.toString());
        s.total = data.total;

        const tbody = $('shop-product-body');
        tbody.innerHTML = '';
        if (!data.items || !data.items.length) {
            tbody.appendChild(emptyRow(7));
        } else {
            for (const d of data.items) {
                const tr = document.createElement('tr');
                const published = !!firstOf(d, ['Published', 'published']);
                const mode = Number(firstOf(d, ['SaleMode', 'saleMode'])) || 1;

                tr.appendChild(cell(firstOf(d, ['ProductId', 'productId']), 'mono'));
                tr.appendChild(cell(firstOf(d, ['Name', 'name'])));
                tr.appendChild(cell(shopModeText(mode), 'dim'));
                tr.appendChild(cell(
                    mode === 2
                        ? `起拍 ${fmt(firstOf(d, ['StartPrice', 'startPrice']))} / 加价 ${fmt(firstOf(d, ['MinIncrement', 'minIncrement']))}`
                        : fmt(firstOf(d, ['Price', 'price'])),
                ));
                tr.appendChild(cell(
                    mode === 2
                        ? (Number(firstOf(d, ['AuctionDurationSeconds', 'auctionDurationSeconds'])) || 0) + ' 秒'
                        : (firstOf(d, ['Stock', 'stock']) === -1 ? '不限' : fmt(firstOf(d, ['Stock', 'stock']))),
                    'dim',
                ));

                const statusTd = document.createElement('td');
                const badge = document.createElement('span');
                badge.textContent = published ? '已上架' : '未上架';
                badge.className = published ? 'tag ok' : 'tag';
                statusTd.appendChild(badge);
                tr.appendChild(statusTd);

                const actTd = document.createElement('td');
                const id = firstOf(d, ['ProductId', 'productId']);
                actTd.appendChild(actionButton('编辑', () => openShopProductModal(d)));
                if (published) {
                    actTd.appendChild(actionButton('下架', () => unpublishShopProduct(id)));
                } else {
                    actTd.appendChild(actionButton('上架', () => publishShopProduct(id)));
                }
                actTd.appendChild(actionButton('删除', () => deleteShopProduct(id)));
                tr.appendChild(actTd);

                tbody.appendChild(tr);
            }
        }

        renderPager('shop-product-page-info', 'shop-product-prev', 'shop-product-next',
            'shopProducts', loadShopProducts);
    } catch (e) {
        notify('商品查询失败：' + e.message, true);
    }
}

function openShopProductModal(doc) {
    editingShopProductId = doc ? firstOf(doc, ['ProductId', 'productId']) : null;
    $('shop-modal-title').textContent = doc ? '编辑商品' : '新建商品';
    $('sp-id').value = editingShopProductId || '';
    $('sp-id').disabled = !!doc;
    $('sp-name').value = doc ? firstOf(doc, ['Name', 'name']) : '';
    $('sp-category').value = doc ? firstOf(doc, ['Category', 'category']) : '';
    $('sp-icon').value = doc ? firstOf(doc, ['Icon', 'icon']) : '';
    $('sp-mode').value = String(doc ? (Number(firstOf(doc, ['SaleMode', 'saleMode'])) || 1) : 1);
    $('sp-currency').value = doc ? firstOf(doc, ['CurrencyItemId', 'currencyItemId']) : '';
    $('sp-price').value = doc ? Number(firstOf(doc, ['Price', 'price'])) || 0 : 0;
    $('sp-start-price').value = doc ? Number(firstOf(doc, ['StartPrice', 'startPrice'])) || 0 : 0;
    $('sp-min-increment').value = doc ? Number(firstOf(doc, ['MinIncrement', 'minIncrement'])) || 1 : 1;
    $('sp-auction-duration').value = doc
        ? Number(firstOf(doc, ['AuctionDurationSeconds', 'auctionDurationSeconds'])) || 0
        : 0;
    $('sp-stock').value = doc ? Number(firstOf(doc, ['Stock', 'stock'])) : -1;
    $('sp-per-user').value = doc ? Number(firstOf(doc, ['PerUserLimit', 'perUserLimit'])) || 0 : 0;
    $('sp-sort').value = doc ? Number(firstOf(doc, ['Sort', 'sort'])) || 0 : 0;
    $('sp-start-time').value = msToLocalInput(firstOf(doc || {}, ['StartTime', 'startTime']));
    $('sp-end-time').value = msToLocalInput(firstOf(doc || {}, ['EndTime', 'endTime']));
    $('sp-auto-restart').checked = doc ? !!firstOf(doc, ['AutoRestart', 'autoRestart']) : false;
    $('sp-enabled').checked = doc ? !!firstOf(doc, ['Enabled', 'enabled']) : true;
    $('sp-rewards').value = doc ? rewardsToText(doc.Rewards || doc.rewards) : '';
    $('sp-description').value = doc ? firstOf(doc, ['Description', 'description']) : '';
    $('sp-error').textContent = '';
    syncShopModeFields();
    $('shop-product-modal').classList.remove('hidden');
}

function syncShopModeFields() {
    const auction = $('sp-mode').value === '2';
    $('sp-price').disabled = auction;
    $('sp-start-price').disabled = !auction;
    $('sp-min-increment').disabled = !auction;
    $('sp-auction-duration').disabled = !auction;
    $('sp-auto-restart').disabled = !auction;
    $('sp-stock').disabled = auction;
    $('sp-per-user').disabled = auction;
}

async function saveShopProduct() {
    const name = $('sp-name').value.trim();
    if (!name) {
        $('sp-error').textContent = '请填写商品名称';
        return;
    }

    const rewards = parseAttachments($('sp-rewards').value);
    if (!rewards.length) {
        $('sp-error').textContent = '请至少配置一个奖励物品（itemId:数量）';
        return;
    }

    const payload = {
        productId: $('sp-id').value.trim() || null,
        name,
        description: $('sp-description').value.trim(),
        icon: $('sp-icon').value.trim(),
        category: $('sp-category').value.trim(),
        saleMode: parseInt($('sp-mode').value, 10) || 1,
        currencyItemId: $('sp-currency').value.trim(),
        price: parseInt($('sp-price').value || '0', 10),
        startPrice: parseInt($('sp-start-price').value || '0', 10),
        minIncrement: parseInt($('sp-min-increment').value || '1', 10),
        auctionDurationSeconds: parseInt($('sp-auction-duration').value || '0', 10),
        autoRestart: $('sp-auto-restart').checked,
        stock: parseInt($('sp-stock').value || '-1', 10),
        perUserLimit: parseInt($('sp-per-user').value || '0', 10),
        enabled: $('sp-enabled').checked,
        sort: parseInt($('sp-sort').value || '0', 10),
        startTime: localInputToMs($('sp-start-time').value),
        endTime: localInputToMs($('sp-end-time').value),
        rewards: rewards.map((a) => ({ itemId: a.itemId, count: a.count })),
    };

    try {
        const res = editingShopProductId
            ? await putJson('/api/shop/products/' + encodeURIComponent(editingShopProductId), payload)
            : await postJson('/api/shop/products', payload);
        notify(res.warning || '商品已保存');
        $('shop-product-modal').classList.add('hidden');
        await loadShopProducts();
        await loadShopListings();
    } catch (e) {
        $('sp-error').textContent = e.message;
    }
}

async function publishShopProduct(id, force = false) {
    try {
        const url = '/api/shop/products/' + encodeURIComponent(id) + '/publish' + (force ? '?force=true' : '');
        await request(url, { method: 'POST' });
        notify('商品已上架');
        await loadShopProducts();
        await loadShopListings();
    } catch (e) {
        if (!force && /已在售|强制/.test(e.message)) {
            if (confirm(e.message + '\n是否强制覆盖当前在售商品？')) {
                return publishShopProduct(id, true);
            }
            return;
        }
        notify('上架失败：' + e.message, true);
    }
}

async function unpublishShopProduct(id, force = false) {
    if (!force && !confirm('确定下架该商品？已有出价的拍卖会退还给最高出价人。')) {
        return;
    }
    try {
        const url = '/api/shop/products/' + encodeURIComponent(id) + '/unpublish' + (force ? '?force=true' : '');
        const res = await request(url, { method: 'POST' });
        notify(res.warning || '商品已下架');
        await loadShopProducts();
        await loadShopListings();
    } catch (e) {
        if (!force && /强制|出价/.test(e.message)) {
            if (confirm(e.message + '\n是否强制下架（退还最高出价）？')) {
                return unpublishShopProduct(id, true);
            }
            return;
        }
        notify('下架失败：' + e.message, true);
    }
}

async function deleteShopProduct(id) {
    if (!confirm('确定删除商品「' + id + '」？该操作不可恢复。')) {
        return;
    }
    try {
        await request('/api/shop/products/' + encodeURIComponent(id), { method: 'DELETE' });
        notify('商品已删除');
        await loadShopProducts();
        await loadShopListings();
    } catch (e) {
        notify('删除失败：' + e.message, true);
    }
}

// ---------- 线上商品 / 寄拍 ----------

async function loadShopListings() {
    const s = state.shopListings;
    try {
        const q = new URLSearchParams({ page: s.page, pageSize: s.pageSize });
        const source = $('shop-listing-source').value;
        const status = $('shop-listing-status').value;
        if (source) q.set('source', source);
        if (status) q.set('status', status);

        const data = await request('/api/shop/listings?' + q.toString());
        s.total = data.total;

        const tbody = $('shop-listing-body');
        tbody.innerHTML = '';
        if (!data.items || !data.items.length) {
            tbody.appendChild(emptyRow(9));
        } else {
            for (const d of data.items) {
                const tr = document.createElement('tr');
                tr.appendChild(cell(d.listingId, 'mono'));
                tr.appendChild(cell(d.source === 'player' || d.source === 2 ? '玩家寄拍' : '官方', 'dim'));
                tr.appendChild(cell(d.sellerId || '-', 'mono'));
                tr.appendChild(cell(d.title));
                tr.appendChild(cell(shopModeText(d.saleMode), 'dim'));
                tr.appendChild(cell(
                    d.saleMode === 2 || d.saleMode === 'Auction'
                        ? (d.currentPrice > 0 ? fmt(d.currentPrice) + ' / ' + (d.topBidderId || '-') : '起拍 ' + fmt(d.startPrice))
                        : fmt(d.price),
                ));
                tr.appendChild(cell(d.endTime ? fmtTime(d.endTime) : '不限', 'dim'));
                tr.appendChild(cell(shopStatusText(d.status), 'dim'));

                const actTd = document.createElement('td');
                if (d.status === 'active') {
                    actTd.appendChild(actionButton('强制下架', () => cancelShopListing(d.listingId)));
                }
                tr.appendChild(actTd);
                tbody.appendChild(tr);
            }
        }

        renderPager('shop-listing-page-info', 'shop-listing-prev', 'shop-listing-next',
            'shopListings', loadShopListings);
    } catch (e) {
        notify('在售商品查询失败：' + e.message, true);
    }
}

async function cancelShopListing(id) {
    if (!confirm('确定强制下架该 listing？拍卖最高出价会退回给出价人。')) {
        return;
    }
    try {
        await request('/api/shop/listings/' + encodeURIComponent(id) + '/cancel', { method: 'POST' });
        notify('已下架');
        await loadShopListings();
        await loadShopProducts();
    } catch (e) {
        notify('下架失败：' + e.message, true);
    }
}

// ---------- 成交流水 ----------

async function loadShopOrders() {
    const s = state.shopOrders;
    try {
        const q = new URLSearchParams({ page: s.page, pageSize: s.pageSize });
        const data = await request('/api/shop/orders?' + q.toString());
        s.total = data.total;

        const tbody = $('shop-order-body');
        tbody.innerHTML = '';
        if (!data.items || !data.items.length) {
            tbody.appendChild(emptyRow(7));
        } else {
            for (const d of data.items) {
                const tr = document.createElement('tr');
                tr.appendChild(cell(fmtTime(d.createTime), 'dim'));
                tr.appendChild(cell(SHOP_ORDER_TYPE_TEXT[d.type] || d.type, 'dim'));
                tr.appendChild(cell(d.buyerId || '-', 'mono'));
                tr.appendChild(cell(d.sellerId || '-', 'mono'));
                tr.appendChild(cell(fmt(d.price)));
                tr.appendChild(cell(fmt(d.fee)));
                tr.appendChild(cell(d.listingId, 'mono'));
                tbody.appendChild(tr);
            }
        }

        renderPager('shop-order-page-info', 'shop-order-prev', 'shop-order-next',
            'shopOrders', loadShopOrders);
    } catch (e) {
        notify('成交流水查询失败：' + e.message, true);
    }
}

// ---------------- Tab 切换 ----------------

const tabLoaders = {
    overview: loadOverview,
    players: loadPlayers,
    payments: loadPayments,
    gm: loadGmLogs,
    mail: loadMail,
    shop: loadShop,
};

function switchTab(name) {
    if (history.replaceState) history.replaceState(null, '', '#' + name);
    document.querySelectorAll('.tab').forEach((t) => t.classList.toggle('active', t.dataset.tab === name));
    document.querySelectorAll('.panel').forEach((p) => p.classList.toggle('active', p.id === 'tab-' + name));
    if (name === 'overview') {
        loadOverview();
        startOverviewTimer();
    } else {
        stopOverviewTimer();
        tabLoaders[name]();
    }
}

function startOverviewTimer() {
    stopOverviewTimer();
    overviewTimer = setInterval(loadOverview, 5000);
}

function stopOverviewTimer() {
    if (overviewTimer) { clearInterval(overviewTimer); overviewTimer = null; }
}

// ---------------- 初始化 ----------------

function bindEvents() {
    document.querySelectorAll('.tab').forEach((t) => {
        t.addEventListener('click', () => switchTab(t.dataset.tab));
    });

    $('player-search').addEventListener('click', () => { state.players.page = 1; loadPlayers(); });
    $('player-keyword').addEventListener('keydown', (e) => { if (e.key === 'Enter') { state.players.page = 1; loadPlayers(); } });
    $('player-detail-close').addEventListener('click', () => $('player-detail').classList.add('hidden'));
    $('player-detail').addEventListener('click', (e) => { if (e.target === $('player-detail')) $('player-detail').classList.add('hidden'); });

    $('pay-search').addEventListener('click', () => { state.payments.page = 1; loadPayments(); });

    $('gm-submit').addEventListener('click', submitGm);
    $('mail-submit').addEventListener('click', submitMail);

    // 商城
    $('shop-search').addEventListener('click', () => { state.shopProducts.page = 1; loadShopProducts(); });
    $('shop-keyword').addEventListener('keydown', (e) => {
        if (e.key === 'Enter') { state.shopProducts.page = 1; loadShopProducts(); }
    });
    $('shop-status-filter').addEventListener('change', () => { state.shopProducts.page = 1; loadShopProducts(); });
    $('shop-new').addEventListener('click', () => openShopProductModal(null));
    $('shop-modal-close').addEventListener('click', () => $('shop-product-modal').classList.add('hidden'));
    $('shop-product-modal').addEventListener('click', (e) => {
        if (e.target === $('shop-product-modal')) $('shop-product-modal').classList.add('hidden');
    });
    $('sp-save').addEventListener('click', saveShopProduct);
    $('sp-mode').addEventListener('change', syncShopModeFields);
    $('shop-listing-refresh').addEventListener('click', () => {
        state.shopListings.page = 1;
        loadShopListings();
    });
    $('shop-listing-source').addEventListener('change', () => {
        state.shopListings.page = 1;
        loadShopListings();
    });
    $('shop-listing-status').addEventListener('change', () => {
        state.shopListings.page = 1;
        loadShopListings();
    });
}

bindEvents();
const initialTab = (location.hash || '#overview').slice(1);
switchTab(tabLoaders[initialTab] ? initialTab : 'overview');
// 直接打开非大盘页面时也要探活一次，否则右上角一直显示「连接中」
if (initialTab !== 'overview') loadOverview();
