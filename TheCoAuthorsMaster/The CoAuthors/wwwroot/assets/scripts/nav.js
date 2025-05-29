function Logout() {
    window.Logout = async function () {
        const response = await fetch('/api/account/logout', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            window.location.href = '/index';
        } else {
            const error = await response.json();
            alert(error.message || 'Logout failed.');
        }
    };

}