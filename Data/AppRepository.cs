using Microsoft.EntityFrameworkCore;

namespace Kawaiiticker.Data;

public class AppRepository : DbContext {
	protected AppRepository() {
	}

	public AppRepository(DbContextOptions options) : base(options) {
	}

	protected override void OnConfiguring(DbContextOptionsBuilder options) {
		if (options.IsConfigured) return;
		
#if DEBUG
		options.EnableSensitiveDataLogging();
		options.EnableDetailedErrors();
		options.EnableThreadSafetyChecks();
#else
		options.EnableThreadSafetyChecks(false);
#endif
		options.UseSqlite("Data Source=/data/kawiiticker.db");
	}
}