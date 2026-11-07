// Funções nativas de iOS/Android que o código do jogo declara mas o WebGL não tem: viram vazias (retornam 0)
mergeInto(LibraryManager.library, {
  _GameCenterGenerateIdentityVerificationSignature: function () { return 0; },
  _GameCenterGetLocalPlayer: function () { return 0; },
  _GameCenterInit: function () { return 0; },
  _GameCenterLoadAchievementDescriptions: function () { return 0; },
  _GameCenterLoadAchievements: function () { return 0; },
  _GameCenterLoadFriends: function () { return 0; },
  _GameCenterLoadLeaderboard: function () { return 0; },
  _GameCenterLoadLeaderboardForIDs: function () { return 0; },
  _GameCenterLoadPhoto: function () { return 0; },
  _GameCenterLoadPlayers: function () { return 0; },
  _GameCenterReportAchievement: function () { return 0; },
  _GameCenterReportAchievements: function () { return 0; },
  _GameCenterReportScore: function () { return 0; },
  _GameCenterResetAchievements: function () { return 0; },
  _GameCenterShowLoginView: function () { return 0; },
  _GameCenterShowView: function () { return 0; },
  adBridgeMethodInvokerCallMethodOnAdBridge: function () { return 0; },
  adBridgeMethodInvokerCallMethodOnAdBridgeWithParams: function () { return 0; },
  adBridgeMethodInvokerCallMethodOnAdBridgeWithReturnValue: function () { return 0; },
  adBridgeMethodInvokerCallMethodOnAdBridgeWithReturnValueAndParams: function () { return 0; },
  adBridgeMethodInvokerCreateAdBridge: function () { return 0; },
  // Recarrega a página depois de um tempo (dá tempo das PlayerPrefs irem para o IndexedDB)
  GjReloadPage: function (delayMs) { setTimeout(function () { location.reload(); }, delayMs); },
  // Qualidade gráfica do ambiente: 0 = Baixa, 1 = Alta (padrão). Valores antigos (Média/Alta) viram Alta.
  GjGetGraphics: function () {
    try { return localStorage.getItem("gj-graficos") === "0" ? 0 : 1; } catch (e) { return 1; }
  },
  // Modo do site: ?mode=original = jogo sem os botões de treino (igual ao ashuni.lol)
  GjIsOriginalMode: function () {
    try { return /[?&]mode=original\b/.test(location.search) ? 1 : 0; } catch (e) { return 0; }
  },
  // Salva e recarrega: o cenário é montado com a qualidade escolhida quando o jogo abre.
  // A espera deixa o save/PlayerPrefs terminarem de gravar no IndexedDB.
  GjSetGraphics: function (level) {
    try { localStorage.setItem("gj-graficos", level === 0 ? "0" : "1"); } catch (e) {}
    setTimeout(function () { location.reload(); }, 700);
  },
});
