<script src="/lib/js/bootstrap.bundle.min.js"></script>
<script src="/lib/js/jquery-3.6.0.min.js"></script>
<script src="/lib/js/apicall.js"></script>
<script src="https://kit.fontawesome.com/154158c4b0.js" crossorigin="anonymous"></script>

<nav class="navbar navbar-expand-lg navbar-light bg-light">
  <div class="container-fluid">
    <a class="navbar-brand" href="/">O3C Manager</a>
    <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
      <span class="navbar-toggler-icon"></span>
    </button>
    <div class="collapse navbar-collapse" id="navbarNav">
      <ul class="navbar-nav">
        <li class="nav-item">
          <a class="nav-link <?=$GLOBALS["activeLink"] == "servers" ? "active" : ""?>" aria-current="page" href="/servers">Servers</a>
        </li>
        <li class="nav-item">
          <a class="nav-link <?=$GLOBALS["activeLink"] == "folders" ? "active" : ""?>" aria-current="page" href="/folders">Folders</a>
        </li>
        <li class="nav-item">
          <a class="nav-link <?=$GLOBALS["activeLink"] == "register" ? "active" : ""?>" aria-current="page" href="/register">Register</a>
        </li>
      </ul>
    </div>
  </div>
</nav>