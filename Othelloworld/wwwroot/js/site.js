// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const Admin = (() => {

	const configMap = {

	}

	const init = () => {

	}

	return {
		init
	};
})();

Admin.Form = (() => {

	class Form {
		constructor(id) {
			this.id = id;
			this.inputs = this.findInputs(id);

			this.findInputButtons(this.inputs);
			this.findConfirmButton(id);
		}

		findInputs(id) {
			return $(`#${id}`).find('input[type').not(':input[type=submit]')
		}

		findInputButtons(inputs) {
			$(inputs).each((_, input) => {
				const inputId = $(input).attr('id');
				const buttonId = $(input).siblings(`button`)
					.first()
					.attr('id');

				if (buttonId.endsWith('_edit_button')) {
					this.createEditButton(inputId, buttonId);
				} else if (buttonId.endsWith('_reset_button')) {

				}
			})
		}

		createEditButton(inputId, buttonId) {
			const input = $(`#${inputId}`);
			const button = $(`#${buttonId}`);

			button.on("click", (event) => {
				event.preventDefault();

				let disabled = input.attr("disabled");

				button.text(disabled ? "Save" : "Edit");
				input
					.attr("disabled", !disabled)
					.focus();

				return false;
			});

			input.on("blur", () => {
				input.attr("disabled", true);
			})
		}

		findConfirmButton(id) {
			$(`#${id}`).submit((event) => {

				const submitterId = $(event.originalEvent.submitter).attr('id');

				if (submitterId === 'confirm') {
					return true;
				} else {
					event.preventDefault();
					return false;
				}
			});
		}
	}


	const register = (id) => {
		return new Form(id);
	}

	return {
		register
	};
})();