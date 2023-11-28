
        function showSlider()
        {
            var btnCon = document.getElementById("buttonContainer");
            btnCon.style.display = "none";
            btnCon.children.add

            var slider = document.getElementById("slider");
            slider.style.display = "block";
            slider.querySelector("input").disabled = false;
        }

        (function() {
        var slider, sliderInput, sliderButton, sliderText, sliderTimeout, sliderOnchange, unlockCheck, resetTimeout;


        slider = document.querySelector('.alarm-slider');
        sliderInput = slider.querySelector('input');
        sliderButton = sliderInput.querySelector('::-webkit-slider-thumb');
        sliderText = slider.querySelector('span');

        unlockCheck = function() {
            if(sliderInput.value == 100) {
                sliderText.innerHTML = 'Larm skickat!';
                sliderInput.value = 0;
                sliderText.style.opacity = 1;

                sliderInput.disabled = true;
                sliderInput.style.display = "none";
                sliderText.style.display = "none";
                var text = document.createElement("p");
                text.style.color = "red";
                text.style.fontSize = "150%";
                //text.innerHTML = "Alarm skickat!";

                slider.appendChild(text);
                onSlide(text);

            } else {
                clearTimeout(resetTimeout);
                resetTimeout = setTimeout(function(){
                    sliderInput.value = 0;
                    sliderText.style.opacity = 1;
                }, 1000);
            }
        };

        sliderOnchange = function() {
            sliderText.style.opacity = ((100 - sliderInput.value) / 200);

            unlockCheck();
        }

        function onSlide(text)
        {
            var xhttp = new XMLHttpRequest();
            xhttp.onreadystatechange = function()
            {
                if (this.readyState == 4 && this.status == 200) {
                    var response = this.responseText;

                    text.innerHTML = response;
                }
            };
            text.innerHTML = "Kontaktar larmcentral...";
            var params = "action=alarm&uid=" + uid;
            xhttp.open("POST", "../api/caseaction.php", true);
            xhttp.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
            xhttp.send(params);
        }


        slider.onchange = sliderOnchange;
        slider.oninput = sliderOnchange;
        slider.onblur = function()
        {
            sliderInput.value = 0;
        };

        })();

        function acknowledge()
        {
            var xhttp = new XMLHttpRequest();
            xhttp.onreadystatechange = function()
            {
                if (this.readyState == 4 && this.status == 200) {
                    var response = this.responseText;
                    ack = response;

                    var ackBtn = document.getElementById("ackBtn");
                    console.log(ack);

                    if(ack == "1")
                    {
                        ackBtn.innerHTML = "Okvittera";
                        ackBtn.classList.remove("btn-primary");
                        ackBtn.classList.add("btn-outline-secondary");
                    }
                    else
                    {
                        ackBtn.innerHTML = "Kvittera";
                        ackBtn.classList.add("btn-primary");
                        ackBtn.classList.remove("btn-outline-secondary");
                    }
                }
            };
            var action = "acknowledge";
            if(ack == "1")
            {
                action = "unacknowledge";
            }
            //var action = ack == "0" ? : "acknowledge" : "unacknowledge";
            var params = "action=" + action + "&uid=" + uid;
            xhttp.open("POST", "../api/caseaction.php", true);
            xhttp.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
            xhttp.send(params);
        }