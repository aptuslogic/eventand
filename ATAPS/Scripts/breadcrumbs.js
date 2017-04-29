//ega what if sessionStorage is not defined?  maybe just don't show breadcrumbs, what browser to test this with?

Storage.prototype.setObj = function (key, obj)
{
    return (this.setItem(key, JSON.stringify(obj)));
}

Storage.prototype.getObj = function (key)
{
    return (JSON.parse(this.getItem(key)));
}

$.fn.extend({

    breadcrumbs: function () {

        // set styling
        $(this).css("margin-top","10px").css("margin-bottom","10px").css("font-size","10pt");

        // manage the trail
        $(this).breadcrumbs_manage ();

        // display the trail
        $(this).html($(this).breadcrumbs_get ());

    },

    breadcrumbs_manage: function () {

        // set current crumb
        var title = document.title.replace (" - ATAP System", "");//ega
        var new_crumb = {url: window.location.href, name: title};

        // see if this crumb is in the list already
        var found = false;
        var crumbs = sessionStorage.getObj("breadcrumbs");
        if (crumbs)
        {
            $.each(crumbs, function (index, crumb) {
                if (!found && crumb.url == new_crumb.url)
                {
                    crumbs.splice(index + 1);
                    sessionStorage.setObj("breadcrumbs", crumbs);
                    found = true;
                    return;
                }
            });

            // append the new crumb
            if (!found)
            {
                var i = crumbs.length;
                crumbs[i] = new_crumb;
                sessionStorage.setObj("breadcrumbs", crumbs);
            }
        }
        else
        {
            // create the first crumb
            sessionStorage.setObj("breadcrumbs", [new_crumb]);
        }

    },

    breadcrumbs_get: function () {
        var str = "<strong>Navigation:</strong> ";
        var crumbs = sessionStorage.getObj("breadcrumbs");
        var separator = "";
        $.each(crumbs, function (index, crumb) {
            str += separator + "<a href=\"" + crumb.url + "\">" + crumb.name + "</a>";
            separator = " &gt;&gt; ";
        });
        return (str);
    }

});