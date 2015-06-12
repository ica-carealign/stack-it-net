using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Castle.Core.Internal;

using Ica.StackIt.Application.Configuration;
using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;
using Ica.StackIt.Interactive.WebPortal.Models.Product;

using Microsoft.Ajax.Utilities;

using Version = Ica.StackIt.Core.Entities.Version;

namespace Ica.StackIt.Interactive.WebPortal.Controllers
{
	[Authorize]
	[SessionProfile]
	public class ProductController : Controller
	{
		private readonly IRepository<Product> _productRepository;
		private readonly IRepository<BaseImage> _baseImageRepository;
		private readonly IStackItConfiguration _stackItConfiguration;

		public ProductController(
			IRepository<Product> productRepository,
			IRepository<BaseImage> baseImageRepository,
			IStackItConfiguration stackItConfiguration)
		{
			_productRepository = productRepository;
			_baseImageRepository = baseImageRepository;
			_stackItConfiguration = stackItConfiguration;
		}

		// GET: Product
		public ActionResult Index()
		{
			List<ProductSummaryViewModel> products = _productRepository.FindAll()
				.OrderBy(x => x.Name)
				.Select(x => new ProductSummaryViewModel
			{
				ProductId = x.Id,
				ProductName = x.Name,
				ProductVersions = x.Versions.Select(ver => ver.Name)
			}).ToList();

			return View(products);
		}

		// GET: Product/Details/5
		public ActionResult Details(Guid id)
		{
			Product product = _productRepository.Find(id);
			return View(product);
		}

		// GET: Product/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: Product/Create
		[HttpPost]
		public ActionResult Create(ProductViewModel model)
		{
			try
			{
				if (_productRepository.FindAll().Any(x => x.Name == model.Name))
				{
					ModelState.AddModelError("", string.Format("A product with the name {0} already exists", model.Name));
					return View();
				}

				if(model.Name.IsNullOrEmpty())
				{
					ModelState.AddModelError("", string.Format("The product name is required."));
					return View();
				}

				var product = new Product
				{
					Name = model.Name
				};

				_productRepository.Add(product);

				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}

		// GET: Product/Edit/5
		public ActionResult Edit(Guid id)
		{
			var product = _productRepository.Find(id);
			var productViewModel = new ProductViewModel {Id = id, Name = product.Name};
			return View(productViewModel);
		}

		// POST: Product/Edit/5
		[HttpPost]
		public ActionResult Edit(ProductViewModel model)
		{
			try
			{
				var product = _productRepository.Find(model.Id);
				product.Name = model.Name;
				_productRepository.Update(product);

				return RedirectToAction("Index");
			}
			catch
			{
				ModelState.AddModelError("", "Could not update product.");
				return View();
			}
		}

		// GET: Product/Delete/5
		public ActionResult Delete(Guid id)
		{
			var product = _productRepository.Find(id);
			var model = new ProductViewModel {Id = id, Name = product.Name};
			return View(model);
		}

		// POST: Product/Delete/5
		[HttpPost]
		public ActionResult Delete(ProductViewModel model)
		{
			try
			{
				_productRepository.Delete(model.Id);
				return RedirectToAction("Index");
			}
			catch
			{
				ModelState.AddModelError("", string.Format("Unable to delete product {0}", model.Id));
				return View("delete");
			}
		}

		// GET: /Product/AddVersion?ProductId=<id>
		[HttpGet]
		public ActionResult AddVersion(Guid productId, string error = null)
		{
			if(error != null)
			{
				ModelState.AddModelError("", error);
			}

			var model = new VersionViewModel {ProductId = productId};
			return View(model);
		}

		[HttpPost]
		public ActionResult AddVersion(VersionViewModel version)
		{
			string error = null;
			if(TryValidateModel(version))
			{
				Product product = _productRepository.Find(version.ProductId);
				product.Versions.Add(new Version { Name = version.Name, IamRole = version.IamRole});
				_productRepository.Update(product);
			}
			else
			{
				error = ModelState.SelectMany(x => x.Value.Errors).First().ErrorMessage;
			}

			return RedirectToAction("Index", new {productId = version.ProductId, error});
		}

		// GET: /Product/AddRoles/ProductId=<id>&Version=<name>
		[HttpGet]
		public ActionResult AddRoles(Guid productId, string version, string error = null)
		{
			if(error != null)
			{
				ModelState.AddModelError("", error);
			}

			var product = _productRepository.Find(productId);

			var currentRoles = product.Versions.Single(x => x.Name == version).Roles.Select(x => x.Name);

			var baseImages = _baseImageRepository.FindAll().ToDictionary(x => x.Id, x => x.Name);
			var model = new RoleViewModel
			{
				ProductId = productId,
				ProductName = product.Name,
				VersionName = version,
				BaseImages = baseImages,
				Ports = new List<PortViewModel> {  new PortViewModel { DnsTtl = 3600}},
				CurrentRoles = currentRoles.ToList(),
				VolumeSize = 60,
				InstanceTypes = _stackItConfiguration.InstanceTypes.Select(x => x.Name).ToList()
			};

			return View(model);
		}

		[HttpPost]
		public ActionResult AddRoles(RoleViewModel roleViewModel)
		{
			if(!TryValidateModel(roleViewModel))
			{
				var error = ModelState.SelectMany(x => x.Value.Errors).FirstOrDefault();
				string errorMessage = null;
				if(error != null)
				{
					errorMessage = error.ErrorMessage;
				}
				return RedirectToAction("AddRoles", new {productId = roleViewModel.ProductId, version = roleViewModel.VersionName, error = errorMessage});
			}

			var product = _productRepository.Find(roleViewModel.ProductId);
			var baseImage = _baseImageRepository.Find(roleViewModel.BaseImageId);

			var newRole = new Role
			{
				Name = roleViewModel.Name,
				BaseImageId = baseImage.Id,
				Ports = roleViewModel.Ports.Select(port => new Port
				{
					Clusterable = port.Clusterable,
					External = port.External,
					Inbound = port.Inbound,
					Outbound = port.Outbound,
					PortNumber = port.PortNumber,
					Provides = port.Provides,
					Tcp = port.Tcp,
					Udp = port.Udp,
					ResourceRecords = port.DnsName.IsNullOrWhiteSpace() ? new List<ResourceRecord>() : new List<ResourceRecord>
					{
						new ResourceRecord { TimeToLive = port.DnsTtl, Type = "NS", Values = new List<string>{port.DnsName}}
					}
				}).ToList(),
				Options = new RoleOptions
				{
					VolumeType = roleViewModel.SelectedDiskType,
					InstanceType = roleViewModel.SelectedInstanceType,
					VolumeSize = roleViewModel.VolumeSize,
					Schedule = roleViewModel.Schedule
				}
			};

			product.Versions.Single(x => x.Name == roleViewModel.VersionName).Roles.Add(newRole);

			_productRepository.Update(product);

			return RedirectToAction("AddRoles", new {roleViewModel.ProductId, version = roleViewModel.VersionName});
		}
	}
}