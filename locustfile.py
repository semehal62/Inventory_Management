from locust import HttpUser, task, between


class InventoryUser(HttpUser):
    wait_time = between(1, 3)

    def on_start(self):
        # Test account credentials
        username = "Mesi"
        password = "1234"

        response = self.client.post(
            "/Auth/login",
            json={
                "username": username,
                "password": password
            },
            verify=False,
            name="LOGIN"
        )

        if response.status_code == 200:
            # AuthController returns the JWT as plain text
            self.token = response.text.strip()
            print("Login successful")
        else:
            self.token = None
            print(
                f"Login failed: "
                f"{response.status_code} - {response.text}"
            )

    def auth_headers(self):
        return {
            "Authorization": f"Bearer {self.token}"
        }

    # =========================
    # SALES
    # =========================

    @task(3)
    def get_sales(self):
        self.client.get(
            "/Sales/GetAll",
            verify=False,
            name="GET Sales"
        )

    # =========================
    # MANAGERS
    # =========================

    @task(2)
    def get_managers(self):
        self.client.get(
            "/Manager/GetAll",
            verify=False,
            name="GET Managers"
        )

    # =========================
    # AUDIT LOGS
    # =========================

    @task(2)
    def get_audits(self):
        self.client.get(
            "/Audits_log/GetAll",
            verify=False,
            name="GET Audit Logs"
        )

    # =========================
    # ITEMS
    # =========================

    @task(3)
    def get_items(self):
        if self.token:
            self.client.get(
                "/Items/GetAll",
                headers=self.auth_headers(),
                verify=False,
                name="GET Items"
            )

    # =========================
    # EMPLOYEES
    # =========================

    @task(2)
    def get_employees(self):
        if self.token:
            self.client.get(
                "/Employee/GetAll",
                headers=self.auth_headers(),
                verify=False,
                name="GET Employees"
            )