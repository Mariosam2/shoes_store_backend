const eyes = document.querySelectorAll(".login .input-container .eye");
const eyeOpen = document.querySelector(".login .input-container .eye.open");
const eyeClosed = document.querySelector(".login .input-container .eye.closed");
const inputs = document.querySelectorAll(".login .input-container input");
const errorMessage = document.querySelector(".login .error-message");

eyes.forEach((eye) => {
  eye.addEventListener("click", () => {
    const passwordInput = document.querySelector(".login .input-container input#password");
    eyeOpen.classList.toggle("hidden");
    eyeClosed.classList.toggle("hidden");
    passwordInput.type = passwordInput.type === "password" ? "text" : "password";
  });
});

inputs.forEach((input) => {
  input.addEventListener("input", () => {
    errorMessage.remove();
  });
});
