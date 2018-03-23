import $ from 'jquery';
import { serializeData } from '../utils/forms';
import URL from 'url-parse';

function getDestinationURL(destination, roomID) {
    const url = new URL(destination);
    url.query = {
        roomID
    };
    return url.toString();
}

function setFormDestinationValue($form, destination) {
    $form.find("[name=destination]").val(destination);
}

function submitForm($form) {
    const data = serializeData($form);

    if(!data.roomID) {
        return Promise.reject(new Error("Tried to submit form with no room ID."));
    }

    const url = getURL(data.destination, data.roomID);
    location.href = url;

    return Promise.resolve();
}

function setupSelectRoomModal() {
    const $modal = $('#select-room-modal');
    const $form = $modal.find('form').first();

    $form.on('submit', function(e){
        e.preventDefault();

        submitForm($form)
            .then(() => {
                $modal.modal('hide')
            })
            .catch((e) => {
                console.log(e.message);
            });
    })

    $modal.on('show.bs.modal', function(e){
        if(!e.relatedTarget) {
            return;
        }

        const $trigger = $(e.relatedTarget);
        const {destination} = $trigger.data();
        setFormDestinationValue($form, destination);
    });
}

export default setupSelectRoomModal;