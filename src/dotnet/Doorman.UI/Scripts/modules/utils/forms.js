export function serializeData($form) {
    return $form.serializeArray().reduce(function(data, x){
        data[x.name] = x.value;

        return data;
    }, {});
}
