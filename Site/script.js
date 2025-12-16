

//#region newSquad

const names = ["Abdullah", "Aalee", "Mohamed", "Sultan", "Khaled"];

function newSquad() {
  const container = document.getElementsByClassName("container")[0];

  const squad_container = document.createElement("div");
  squad_container.className = "squad_container";

  host_name = names[Math.floor(Math.random() * names.length)]

  squad_container.innerHTML = `
  
                    <div class='content'>
                        <div class='top_section'>
                            <div class='mission_info'>
                                Survival <br>

                                <div class="relic">Axi A1</div>
                            </div>
                            <div class='players_count'><i class="fa-regular fa-user"></i> 1/4</div>
                        </div>

                        <div style="font-size: 12px;" class='host_name'><span
                                style="color: #90A1B9;">Host:
                            </span>${host_name}</div>
                    </div>

                    <button onclick="joinSquad(this)">Join Squad</button>
  `;

  squad_container.dataset.host_name = host_name


  container.appendChild(squad_container);
}


//#region joinSquad

function joinSquad(button) {
  const squad = button.closest(".squad_container");
  const host_name = squad.dataset.host_name;

  fetch("/join", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ host_name })
  });
}

for (let i = 0; i < 51; i++) {
  // newSquad();
}