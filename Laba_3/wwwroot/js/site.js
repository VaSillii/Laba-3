// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function updateSize() {
    var mb_in_bytes = 1024 * 1024;
    var file = document.getElementById("data-file").files[0],
        type_file = file.name.split('.').pop();
    if (file.size < mb_in_bytes * 5 && type_file === "docx") {
        return true;
    }
    return false;
}

function is_empty(x) {
    return (
        (typeof x == 'undefined')
        ||
        (x == null)
        ||
        (x == false)  //same as: !x
        ||
        (x.length == 0)
        ||
        (x == "")
        ||
        (x.replace(/\s/g, "") == "")
        ||
        (!/[^\s]/.test(x))
        ||
        (/^\s*$/.test(x))
    );
}

let input = document.querySelector('#key');

input.addEventListener('input', () => {
    input.value = input.value.replace(/[^а-яА-ЯёЁ ]/, '');
});

var form = document.getElementById('form').addEventListener('submit', function (event) {
    event.preventDefault();
    var key = document.getElementById("key");

    var file = document.getElementById("data-file");
    var textarea = document.getElementById("inputText");
    var error = false;


    if (is_empty(textarea.value) && !file.value) {
        textarea.classList.add('is-invalid');
        file.classList.add('is-invalid');
        error = true;
    } else {
        if (!is_empty(textarea.value)) {
            textarea.classList.add('is-valid');
        }

        if (file.value) {
            if (!updateSize()) {
                file.classList.add('is-invalid');
                error = true;
            } else {
                file.classList.remove('is-invalid');
                file.classList.add('is-valid');
            }
        }
    }

    if (!error) {
        this.submit();
    }
});
