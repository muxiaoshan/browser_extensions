chrome.runtime.onMessageExternal.addListener(
    function (request, sender, sendResponse) {
        
        console.debug("监听页面发送来的消息");
        console.debug(request);
        console.debug(sender);
        if (request && request.operation == "closeCurrentTab") {
            var tabId = sender.tab.id;
            chrome.tabs.remove(tabId);
        }
        sendResponse({
            success: true,
            closed: true,
        });
        //return true;
    }
);