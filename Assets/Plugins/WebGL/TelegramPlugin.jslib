mergeInto(LibraryManager.library, {
    GetUserName: function() {
        var urlParams = new URLSearchParams(window.location.search);
        var userName = urlParams.get('username');
        if (!userName) {
            userName = "User not available";
        }
        return allocate(intArrayFromString(userName), ALLOC_NORMAL);
    }
});
