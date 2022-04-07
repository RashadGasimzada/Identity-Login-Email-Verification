$(document).ready(function () {

    $(document).on("click", ".set-default", function () {


        let ProductId = parseInt($(".product-id").val());
        let ImageId = parseInt($(this).attr("data-id"));

        $.ajax({
            // url: "/Product/LoadMore?skip" + count,
            url: "/AdminArea/Product/SetDefaultImage",
            data: {
                productId:ProductId,
                imageId:ImageId
            },
            type: "Post",
            success: function (res) {

                Swal.fire({
                    position: 'top-end',
                    icon: 'success',
                    title: 'Image default success',
                    showConfirmButton: false,
                    timer:1500


                }).then(function () {
                    window.location.reload();
                })

            }

        })


    })







})

   



