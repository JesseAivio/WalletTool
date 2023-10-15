// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.getElementById('amount').addEventListener('input', function (e) {
    if (e.target.value.startsWith('0')) {
        e.target.value = '0';
    } else {
        e.target.value = e.target.value.replace(/[^\d]/g, '');
    }
});
document.getElementById('price').addEventListener('input', function (e) {
    if (e.target.value.startsWith('0')) {
        e.target.value = '0';
    } else {
        e.target.value = e.target.value.replace(/[^\d]/g, '');
    }
});
function convertToInputDateFormat(dateTimeStr) {
    const [datePart] = dateTimeStr.split(' ');

    const dateSegments = datePart.split('.');

    if (dateSegments.length !== 3) {
        throw new Error('Invalid datetime format');
    }

    return `${dateSegments[2]}-${String(dateSegments[1]).padStart(2, '0')}-${String(dateSegments[0]).padStart(2, '0')}`;
}
function editTransaction(id, name, price, amount, date, type){
    alert(`${id}, ${name}, ${price}, ${amount}, ${convertToInputDateFormat(date)}, ${type},`)
    document.getElementById('id').value = id;
    document.getElementById('name').value = name;
    document.getElementById('price').value = price;
    document.getElementById('amount').value = amount;
    document.getElementById('date').value = convertToInputDateFormat(date);
    if(type === "Income"){
        document.getElementById('type').value = "0";
    }else{
        document.getElementById('type').value = "1";
    }
    
}

function addTransaction() {
    document.getElementById('id').value = '';
    document.getElementById('name').value = '';
    document.getElementById('price').value = '';
    document.getElementById('amount').value = '';
    document.getElementById('date').value = '';
    document.getElementById('type').value = 0;
}
  // Handle form submission
    document.getElementById('transactionForm').addEventListener('submit', function(e) {
        e.preventDefault();

        // Check if the form is valid
        if (this.checkValidity() === false) {
            e.stopPropagation();
            // Add `was-validated` class to show errors
            this.classList.add('was-validated');
            return; // Exit the event listener without executing the rest of the code
        }
        
        const formData = new FormData(this);
        const isUpdating = document.getElementById('id').value !== '';
        const url = isUpdating ? "/Transactions/UpdateTransaction" : "/Transactions/AddTransaction";
        var transaction = {
            name: document.getElementById('name').value,
            price: document.getElementById('price').value,
            amount: document.getElementById('amount').value,
            date: document.getElementById('date').value,
            type: document.getElementById('type').value,
        }
        console.log(formData);
        console.log(transaction);
        fetch(url, {
            method: 'POST',
            body: formData
        })
            .then(response => response.json())
            .then(data => {
                // Handle successful response (e.g., refresh the page or update the table directly)
                alert('OK')
            })
            .catch(error => {
                alert(error);
                console.error('Error:', error);
            });
    });

function applyFilter() {
    var year = document.getElementById("yearFilter").value;
    var month = document.getElementById("monthFilter").value;
    var url = 'Transactions?currentPage=1';

    if (year) {
        url += '&filterYear=' + year;
    }
    if (month) {
        url += '&filterMonth=' + month;
    }

    window.location.href = url;
}
