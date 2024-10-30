function initFormTransactions(urlGetCategories) {
    $("#OperationTypeId").on("change", async function () {
        const valSelected = $(this).val();

        const response = await fetch(urlGetCategories, {
            method: 'POST',
            body: valSelected,
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const json = await response.json();
        const options = json.map(category => `<option value=${category.value}>${category.text}</option>`);
        $('#CategoryId').html(options)

    })
}