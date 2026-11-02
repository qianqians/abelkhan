'use strict';

const $ = (id) => document.getElementById(id);

const state = {
    players: { page: 1, pageSize: 20 },
    payments: { page: 1, pageSize: 20 },
    gm: { page: 1, pageSize: 20 },
    mail: { page: 1, pageSize: 20 },
};

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
            tbody.appendChild(emptyRow(4));
        } else {
            for (const d of data.items) {
                const tr = document.createElement('tr');
                tr.appendChild(cell(mailRecipients(d), 'mono'));
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
    const playerIds = $('mail-user-id').value
        .split(/[\s,，;；]+/)
        .map((s) => s.trim())
        .filter(Boolean);
    try {
        const res = await postJson('/api/mail/send', {
            title,
            content,
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
        state.mail.page = 1;
        loadMail();
    } catch (e) {
        notify('发送失败：' + e.message, true);
    }
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

// ---------------- Tab 切换 ----------------

const tabLoaders = {
    overview: loadOverview,
    players: loadPlayers,
    payments: loadPayments,
    gm: loadGmLogs,
    mail: loadMail,
};

function switchTab(name) {
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
}

bindEvents();
switchTab('overview');
