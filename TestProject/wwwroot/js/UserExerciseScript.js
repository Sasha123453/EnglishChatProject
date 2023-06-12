function openPopup(block) {
    var exerciseBlocks = document.getElementsByClassName("exercise-block");
    for (var i = 0; i < exerciseBlocks.length; i++) {
        exerciseBlocks[i].classList.remove("active");
    }
    block.classList.add("active");

    var details = block.querySelector(".exercise-details");
    var titleElement = block.querySelector(".exercise-name");
    var descriptionElement = details.querySelector(".exercise-description");

    var title = titleElement ? titleElement.textContent : "";
    var description = descriptionElement ? descriptionElement.textContent : "";

    var popupTitle = document.getElementById("popupTitle");
    var popupDescription = document.getElementById("popupDescription");

    popupTitle.innerText = title;
    popupDescription.innerText = description;

    var popupContainer = document.getElementById("popupContainer");
    popupContainer.style.display = "block";
    document.getElementById("Id-toSend").value = block.id;
}

function closePopup() {
    var exerciseBlocks = document.getElementsByClassName("exercise-block");
    for (var i = 0; i < exerciseBlocks.length; i++) {
        exerciseBlocks[i].classList.remove("active");
    }

    var popupContainer = document.getElementById("popupContainer");
    popupContainer.style.display = "none";
}
function checkAnswer() {
    var userInput = $("#answer-text").val();
    var exerciseBlock = $(".exercise-block.active");
    var exerciseModelId = exerciseBlock.data("exercise-model-id");

    $.ajax({
        url: '/Exercise/CheckAnswer',
        type: 'POST',
        data: {
            userInput: userInput,
            exerciseModelId: exerciseModelId
        },
        dataType: 'json',
        success: function (data) {
            exerciseBlock.addClass("solved");
        },
        error: function (xhr) {
            alert(xhr.responseText);
        }
    });
}
function IsExerciseSolved(exerciseId) {
    $.ajax({
        url: '/Exercise/IsExerciseSolved',
        type: 'GET',
        data: { exerciseId: exerciseId },
        dataType: 'json',
        success: function (data) {
            if (data.isSolved) {
                var exerciseBlock = document.getElementById(exerciseId);
                exerciseBlock.classList.add("solved");
            } else {
            }
        },
        error: function (xhr) {
            alert(xhr.responseText);
        }
    });
}